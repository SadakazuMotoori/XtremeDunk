//==================================================================
/// <summary>
/// カメラ管理クラス
/// </summary>
//==================================================================
using SGGames.Game.Sys;

using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 再設計対象
public interface ICameraManager : IService<ICameraManager>
{
    //======================================
    // プレイヤーカメラ関係
    //======================================
    Transform CurrentCameraTarget { get; }
    void SetCameraTarget(Transform target);
    
    CameraController CamCtrl { get; }
    void SetCamera(CameraController CamCtrl);

    float SetCinemachineBrainDefaultBlendTime(float duration);

    Camera UICamera             { get; }
    Camera CurrentMainCamera    { get; }

    // ワールド座標からUI座標へ変換
    Vector2? ConvertWorldToUIPos(Vector3 worldPos) { return Vector2.zero; }

    void ManageMainCamera(ManagedMainCamera camera);
    void UnmanageMainCamera(ManagedMainCamera camera);

    void UpdateManagedMainCameraList();
}

// 他のカメラ系コンポーネントより先にServiceLocatorへ登録したいので、実行順を早めている。
[DefaultExecutionOrder(-10000)]
public class CameraManager : MonoBehaviour, ICameraManager
{
    //======================================
    // プレイヤーカメラ関係
    //======================================
    Transform _currentCameraTarget;
    public Transform CurrentCameraTarget => _currentCameraTarget;
    public void SetCameraTarget(Transform target)
    {
        float old = SetCinemachineBrainDefaultBlendTime(0);
        _currentCameraTarget = target;
        if (_CameraCtrl != null)
        {
            _CameraCtrl.SetCameraTarget(_currentCameraTarget);
        }

        SetCinemachineBrainDefaultBlendTime(old);
    }

    CameraController _CameraCtrl;
    public CameraController CamCtrl => _CameraCtrl;

    public void SetCamera(CameraController CamCtrl)
    {
        _CameraCtrl = CamCtrl;

        // CameraControllerが後から有効化された場合でも、すでに設定済みのターゲットを反映する。
        if (_CameraCtrl != null)
        {
            _CameraCtrl.SetCameraTarget(_currentCameraTarget);
        }
    }

    ReactiveProperty<CameraController.CameraData> _nowCamTypeRP = new();
    public System.IObservable<CameraController.CameraData> NowCamTypeRP => _nowCamTypeRP;

    CameraController.CameraData CurrentCamData
    {
        get
        {
            // ターゲットやControllerが未設定の起動直後は、現在カメラをまだ確定できない。
            if (_currentCameraTarget == null) return null;
            if (_CameraCtrl == null) return null;
            return _CameraCtrl.CurrentNormalCamData;
        }
    }

    // UIカメラ
    Camera _uiCamera;
    public Camera UICamera => _uiCamera;

    [SerializeField] List<Canvas> _uiCanvasList;

    // ワールド->UI座標変換用
    [SerializeField] RectTransform _uiRectTransform;

    //======================================
    // 複数メインカメラ管理
    // ・メインカメラは１つだけ有効にして、他を無効にするため。
    //======================================
    // メインカメラ管理リスト
    HashSet<ManagedMainCamera> _managedMainCameras = new();
    // 現在のメインカメラ
    ManagedMainCamera _currentMainCamera;
    public Camera CurrentMainCamera
    {
        get
        {
            if (_currentMainCamera == null) return null;
            return _currentMainCamera.Cam;
        }
    }
    public ManagedMainCamera CurrentMainCamera2 => _currentMainCamera;

    // ワールド座標からUI座標へ変換
    public Vector2? ConvertWorldToUIPos(Vector3 worldPos)
    {
        // UI座標変換には「現在のメインカメラ」「UIカメラ」「UI基準Rect」がすべて必要。
        if (_currentMainCamera == null) return null;
        if (_currentMainCamera.Cam == null) return null;
        if (_uiRectTransform == null) return null;
        if (_uiCamera == null) return null;

        Vector3 vTarget = worldPos - _currentMainCamera.Cam.transform.position;

        if (Vector3.Dot(vTarget, _currentMainCamera.Cam.transform.forward) < 0) return null;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_currentMainCamera.Cam, worldPos);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_uiRectTransform, screenPos, _uiCamera, out pos);
        return pos;
    }

    // メインカメラ登録
    public void ManageMainCamera(ManagedMainCamera mainCamera)
    {
        // OnEnable/Startの実行順によってnullが来ても、管理リストを壊さない。
        if (mainCamera == null) return;

        _managedMainCameras.Remove(mainCamera);

        _managedMainCameras.Add(mainCamera);

        // 更新
        UpdateManagedMainCameraList();
    }

    // メインカメラ登録解除
    public void UnmanageMainCamera(ManagedMainCamera mainCamera)
    {
        // 破棄中や無効化中にnull扱いになっても、安全に無視する。
        if (mainCamera == null) return;

        _managedMainCameras.Remove(mainCamera);

        // 更新
        UpdateManagedMainCameraList();
    }

    /// <summary>
    /// メインカメラリストの更新処理
    /// ・nullチェック
    /// ・カレントメインカメラ検索＆有効/無効化
    /// </summary>
    public void UpdateManagedMainCameraList()
    {
        _currentMainCamera = null;

        // nullの物を削除
        _managedMainCameras.RemoveWhere(x => x == null);

        // 最優先カメラ検索
        foreach (var managedCam in _managedMainCameras)
        {
            // ManageCamera自体が無効なら、無視する
            if (managedCam.enabled == false)
            {
                continue;
            }

            // Cameraコンポーネントを持たないものは、メインカメラ候補にできない。
            if (managedCam.Cam == null)
            {
                continue;
            }

            // 最も優先順位の高いカメラを残す
            if (_currentMainCamera == null)
            {
                _currentMainCamera = managedCam;
            }
            else if (managedCam.Priority >= _currentMainCamera.Priority)
            {
                _currentMainCamera = managedCam;
            }
        }

        // 最優先カメラのみ有効、その他はすべて無効
        foreach (var managedCam in _managedMainCameras)
        {
            if (_currentMainCamera == managedCam)
            {
                if (managedCam.Cam == null)
                {
                    continue;
                }

                // 最優先のカメラだけを実際に描画する。
                managedCam.Cam.enabled = true;

                var cameraData = _currentMainCamera.Cam.GetUniversalAdditionalCameraData();

                _uiCamera = null;
                if (cameraData != null)
                {
                    // URPのCameraStackからUIレイヤーのカメラを探し、Canvasの描画カメラに使う。
                    int uiLayer = LayerMask.NameToLayer("UI");
                    foreach (var cam in cameraData.cameraStack)
                    {
                        if (cam.gameObject.layer == uiLayer)
                        {
                            _uiCamera = cam;
                        }
                    }
                }

                if (_uiCanvasList != null)
                {
                    foreach (var uiCanvas in _uiCanvasList)
                    {
                        if (uiCanvas != null)
                        {
                            uiCanvas.worldCamera = _uiCamera;
                        }
                    }
                }

            }
            else
            {
                if (managedCam.Cam == null)
                {
                    continue;
                }

                // 優先されなかったメインカメラは無効化し、同時に複数描画されるのを防ぐ。
                managedCam.Cam.enabled = false;
            }
        }
    }

    public float SetCinemachineBrainDefaultBlendTime(float duration)
    {
        // ActiveBrainが1つもない場合、GetActiveBrain(0)自体が失敗するため先に件数を確認する。
        if (Unity.Cinemachine.CinemachineBrain.ActiveBrainCount == 0) return 0;

        Unity.Cinemachine.CinemachineBrain brain = Unity.Cinemachine.CinemachineBrain.GetActiveBrain(0);

        // MainCameraがまだ生成されていないタイミングでは、Blend時間を変更できない。
        if (brain == null) return 0;

        float old = brain.DefaultBlend.Time;
        brain.DefaultBlend.Time = duration;
        return old;
    }

    void Awake()
    {
        // CameraManagerは全体で1つだけ使う。重複した場合は先に登録済みのものを優先する。
        // ServiceLocatorはinterfaceで返すため、Unity独自のnull判定を使えるようObjectとして比較する。
        UnityEngine.Object currentManager = ICameraManager.Instance as UnityEngine.Object;
        if (currentManager != null && currentManager != this)
        {
            Destroy(gameObject);
            return;
        }

        // 他のクラスは ICameraManager.Instance から、このManagerへアクセスする。
        ServiceLocator<ICameraManager>.Register(this);

        // 同じPrefabにCameraControllerが付いている場合は、自動で接続する。
        if (TryGetComponent(out CameraController cameraController))
        {
            SetCamera(cameraController);
        }

        // 同じPrefabにManagedMainCameraが付いている場合は、自分自身をメインカメラ候補にする。
        if (TryGetComponent(out ManagedMainCamera mainCamera))
        {
            ManageMainCamera(mainCamera);
        }
    }

    void OnDestroy()
    {
        // 自分が登録したサービスだけを解除し、別のCameraManagerの登録を消さないようにする。
        // interfaceのまま比較するとUnityのObject比較にならないため、Objectへ戻してから比較する。
        UnityEngine.Object currentManager = ICameraManager.Instance as UnityEngine.Object;
        if (currentManager == this)
        {
            ServiceLocator<ICameraManager>.Unregister();
        }
    }

    void Start()
    {
        // バトルカメラが変更された
        this.ObserveEveryValueChanged(x => _CameraCtrl)
            .Subscribe(ctrl =>
            {
                if (_CameraCtrl != null)
                {
                    _CameraCtrl.SetCameraTarget(_currentCameraTarget);
                }
            });

        // 
        this.ObserveEveryValueChanged(x => CurrentCamData)
            .Subscribe(camData =>
            {
                _nowCamTypeRP.Value = camData;
            });
    }
}
