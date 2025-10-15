//==================================================================
/// <summary>
/// �J�����Ǘ��N���X
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
    // �v���C���[�J�����֌W
    //======================================
    Transform CurrentCameraTarget { get; }
    void SetCameraTarget(Transform target);
    
    CameraController CamCtrl { get; }
    void SetCamera(CameraController CamCtrl);

    float SetCinemachineBrainDefaultBlendTime(float duration);

    Camera UICamera             { get; }
    Camera CurrentMainCamera    { get; }

    // ���[���h���W����UI���W�֕ϊ�
    Vector2? ConvertWorldToUIPos(Vector3 worldPos) { return Vector2.zero; }

    void ManageMainCamera(ManagedMainCamera camera);
    void UnmanageMainCamera(ManagedMainCamera camera);

    void UpdateManagedMainCameraList();
}

public class CameraManager : MonoBehaviour, ICameraManager
{
    //======================================
    // �v���C���[�J�����֌W
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

    // UI�J����
    Camera _uiCamera;
    public Camera UICamera => _uiCamera;

    [SerializeField] List<Canvas> _uiCanvasList;

    // ���[���h->UI���W�ϊ��p
    [SerializeField] RectTransform _uiRectTransform;

    [SerializeField] Transform _virtualCameraParent;

    //======================================
    // �������C���J�����Ǘ�
    // �E���C���J�����͂P�����L���ɂ��āA���𖳌��ɂ��邽�߁B
    //======================================
    // ���C���J�����Ǘ����X�g
    List<ManagedMainCamera> _managedMainCameraList = new();
    HashSet<ManagedMainCamera> _managedMainCameras = new();
    // ���݂̃��C���J����
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

    // ���[���h���W����UI���W�֕ϊ�
    public Vector2? ConvertWorldToUIPos(Vector3 worldPos)
    {
        Vector3 vTarget = worldPos - _currentMainCamera.Cam.transform.position;

        if (Vector3.Dot(vTarget, _currentMainCamera.Cam.transform.forward) < 0) return null;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_currentMainCamera.Cam, worldPos);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_uiRectTransform, screenPos, _uiCamera, out pos);
        return pos;
    }

    // ���C���J�����o�^
    public void ManageMainCamera(ManagedMainCamera mainCamera)
    {
        _managedMainCameras.Remove(mainCamera);

        _managedMainCameras.Add(mainCamera);

        // �X�V
        UpdateManagedMainCameraList();
    }

    // ���C���J�����o�^����
    public void UnmanageMainCamera(ManagedMainCamera mainCamera)
    {
        _managedMainCameras.Remove(mainCamera);

        // �X�V
        UpdateManagedMainCameraList();
    }

    /// <summary>
    /// ���C���J�������X�g�̍X�V����
    /// �Enull�`�F�b�N
    /// �E�J�����g���C���J�����������L��/������
    /// </summary>
    public void UpdateManagedMainCameraList()
    {
        _currentMainCamera = null;

        // null�̕����폜
        _managedMainCameras.RemoveWhere(x => x == null);

        // �ŗD��J��������
        foreach (var managedCam in _managedMainCameras)
        {
            // ManageCamera���̂������Ȃ�A��������
            if (managedCam.enabled == false)
            {
                continue;
            }

            // �ł��D�揇�ʂ̍����J�������c��
            if (_currentMainCamera == null)
            {
                _currentMainCamera = managedCam;
            }
            else if (managedCam.Priority >= _currentMainCamera.Priority)
            {
                _currentMainCamera = managedCam;
            }
        }

        // �ŗD��J�����̂ݗL���A���̑��͂��ׂĖ���
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
        // �o�g���J�������ύX���ꂽ
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
