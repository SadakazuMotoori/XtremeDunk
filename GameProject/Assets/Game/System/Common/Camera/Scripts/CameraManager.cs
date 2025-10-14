using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

using UnityEngine.Rendering.Universal;


public interface ICameraManager// : IService<ICameraManager>
{
/*
    //======================================
    // プレイヤーカメラ関係
    //======================================
    Transform CurrentBattleCameraTarget { get; }
    void SetBattleCameraTarget(Transform target);

    BattleCameraController BattleCamCtrl { get; }
    void SetBattleCamera(BattleCameraController battleCamCtrl);
    void ResetBattleCamRotateAxisToForward(float duration);

    float SetCinemachineBrainDefaultBlendTime(float duration);

    // 視点モードの変更通知
    //    System.IObservable<ICameraManager.CameraTypes> OnBattleCameraTypeChanged { get; }
    //    System.IDisposable SubscribeOnBattleCameraTypeChanged(System.Action<ICameraManager.CameraTypes> onNext);
    System.IObservable<BattleCameraController.CameraData> NowBattleCamTypeRP { get; }
    //    ICameraManager.CameraTypes NowBattleCamType { get; }

    Camera UICamera { get; }

    Camera CurrentMainCamera { get; }

    // ワールド座標からUI座標へ変換
    Vector2? ConvertWorldToUIPos(Vector3 worldPos) { return Vector2.zero; }

    void ManageMainCamera(ManagedMainCamera camera);
    void UnmanageMainCamera(ManagedMainCamera camera);

    void UpdateManagedMainCameraList();
*/
}

//==================================================================
/// <summary>
/// カメラ管理クラス
/// </summary>
//==================================================================
public class CameraManager// : GameMonoBehaviour, ICameraManager//, Cinemachine.AxisState.IInputAxisProvider
{
/*
    //======================================
    // プレイヤーカメラ関係
    //======================================
    Transform _currentBattleCameraTarget;
    public Transform CurrentBattleCameraTarget => _currentBattleCameraTarget;
    public void SetBattleCameraTarget(Transform target)
    {
        float old = SetCinemachineBrainDefaultBlendTime(0);
        _currentBattleCameraTarget = target;
        if (_battleCameraCtrl != null)
        {
            _battleCameraCtrl.SetCameraTarget(_currentBattleCameraTarget);
        }

        SetCinemachineBrainDefaultBlendTime(old);
    }

    BattleCameraController _battleCameraCtrl;
    public BattleCameraController BattleCamCtrl => _battleCameraCtrl;

    public void SetBattleCamera(BattleCameraController battleCamCtrl)
    {
        _battleCameraCtrl = battleCamCtrl;
    }

    public void ResetBattleCamRotateAxisToForward(float duration)
    {
        if (_battleCameraCtrl == null) return;
        _battleCameraCtrl.ResetRotateAxisToForward(duration);
    }

    ReactiveProperty<BattleCameraController.CameraData> _nowBattleCamTypeRP = new();
    public System.IObservable<BattleCameraController.CameraData> NowBattleCamTypeRP => _nowBattleCamTypeRP;


    BattleCameraController.CameraData CurrentCamData
    {
        get
        {
            if (_currentBattleCameraTarget == null) return null;
            return _battleCameraCtrl.CurrentNormalCamData;
        }
    }

    // UIカメラ
    Camera _uiCamera;
    public Camera UICamera => _uiCamera;

    [SerializeField] List<Canvas> _uiCanvasList;

    // ワールド->UI座標変換用
    [SerializeField] RectTransform _uiRectTransform;

    [SerializeField] Transform _virtualCameraParent;


    //======================================
    // 複数メインカメラ管理
    // ・メインカメラは１つだけ有効にして、他を無効にするため。
    //======================================
    // メインカメラ管理リスト
    List<ManagedMainCamera> _managedMainCameraList = new();
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
        _managedMainCameras.Remove(mainCamera);

        _managedMainCameras.Add(mainCamera);

        // 更新
        UpdateManagedMainCameraList();
    }

    // メインカメラ登録解除
    public void UnmanageMainCamera(ManagedMainCamera mainCamera)
    {
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
                managedCam.Cam.enabled = true;

                var cameraData = _currentMainCamera.Cam.GetUniversalAdditionalCameraData();

                _uiCamera = null;
                int uiLayer = LayerMask.NameToLayer("UI");
                foreach (var cam in cameraData.cameraStack)
                {
                    if (cam.gameObject.layer == uiLayer)
                    {
                        _uiCamera = cam;
                    }
                }

                foreach (var uiCanvas in _uiCanvasList)
                {
                    uiCanvas.worldCamera = _uiCamera;
                }

            }
            else
            {
                managedCam.Cam.enabled = false;
            }
        }
    }

    public float SetCinemachineBrainDefaultBlendTime(float duration)
    {
        Unity.Cinemachine.CinemachineBrain brain = Unity.Cinemachine.CinemachineBrain.GetActiveBrain(0);
        float old = brain.DefaultBlend.Time;
        brain.DefaultBlend.Time = duration;
        return old;
    }

    override protected void Start()
    {
        base.Start();

        // バトルカメラが変更された
        this.ObserveEveryValueChanged(x => _battleCameraCtrl)
            .Subscribe(ctrl =>
            {
                if (_battleCameraCtrl != null)
                {
                    _battleCameraCtrl.SetCameraTarget(_currentBattleCameraTarget);
                }
            });

        // 
        this.ObserveEveryValueChanged(x => CurrentCamData)
            .Subscribe(camData =>
            {
                _nowBattleCamTypeRP.Value = camData;
                //                _onBattleCameraTypeChanged.OnNext(type);
            });
    }
*/
}
