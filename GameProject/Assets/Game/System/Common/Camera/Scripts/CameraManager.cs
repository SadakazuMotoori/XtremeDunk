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
    }

    ReactiveProperty<CameraController.CameraData> _nowCamTypeRP = new();
    public System.IObservable<CameraController.CameraData> NowCamTypeRP => _nowCamTypeRP;

    CameraController.CameraData CurrentCamData
    {
        get
        {
            if (_currentCameraTarget == null) return null;
            return _CameraCtrl.CurrentNormalCamData;
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
