//==================================================================
/// <summary>
/// カメラ制御クラス
/// </summary>
//==================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Cysharp.Threading.Tasks;
//using Cysharp.Threading.Tasks.Triggers;

using Unity.Cinemachine;

//using DG.Tweening;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
/*
    // 全カメラデータ
    [SerializeField] CameraData[] _playerVirtualCams;

    // NormalCameraのみのリスト
    List<CameraData> _normalOnlyCams;

    // 
    [Header("※Normal CameraのみのIndex")]
    [SerializeField] int _nowNormalCameraIndex = 2;
    //    public int NowNormalCameraIndex => _nowNormalCameraIndex;

    // NormaCamera専用の現在のカメラ
    public CameraData CurrentNormalCamData => _normalOnlyCams[_nowNormalCameraIndex];

    // 全てのカメラ対象の現在のカメラ
    public ReactiveProperty<CameraData> CurrentCamData { get; set; } = new();

    public CinemachineVirtualCamera CurrentVCam => _playerVirtualCams[_nowNormalCameraIndex].VCam;

    // 
    float _rotateAxisValue_X = 0;
    float _rotateAxisValue_Y = 0;

    bool _stopRotate;

    public bool IsFreeCameraMode => CurrentCamData.Value.CamType == CameraData.CameraTypes.FreeCamera;

    public CameraData GetNormalCameraDataByIndex(int index)
    {
        return _normalOnlyCams[index];
    }


    void Awake()
    {
        DebugLogger.Log("[Test]Awake", DebugLogger.Colors.yellow);
        ICameraManager.Instance.SetBattleCamera(this);

        // 初期設定
        // Normal Cameraのみリストの作成
        _normalOnlyCams = new();
        for (int i = 0; i < _playerVirtualCams.Length; i++)
        {
            _playerVirtualCams[i].Initialize(this);

            if (_playerVirtualCams[i].CamType == CameraData.CameraTypes.NormalCamera)
            {
                _normalOnlyCams.Add(_playerVirtualCams[i]);
            }
        }

    }

    void Start()
    {
        DebugLogger.Log("[Test]Start", DebugLogger.Colors.yellow);
        // 初期設定
        for (int i = 0; i < _playerVirtualCams.Length; i++)
        {
            _playerVirtualCams[i].SetInputAxis();
        }

        // カメラ変更時
        CurrentCamData.Subscribe(camData =>
        {
            if (camData == null) return;

            for (int i = 0; i < _playerVirtualCams.Length; i++)
            {
                if (camData == _playerVirtualCams[i])
                {
                    _playerVirtualCams[i].SetCurrent(true);
                }
                else
                {
                    _playerVirtualCams[i].SetCurrent(false);
                }
            }

        });

        ChangePlayerCamIndex(_nowNormalCameraIndex, 0);

        // 更新処理
        this.UpdateAsObservable()
            .Where(_ => enabled)
            .Subscribe(_ =>
            {
                for (int i = 0; i < _playerVirtualCams.Length; i++)
                {
                    _playerVirtualCams[i].SetInputAxis();
                }

                // 角度同期
                {
                    var nowVCamData = _normalOnlyCams[_nowNormalCameraIndex];
                    float angleY = nowVCamData.GetAngleY();

                    foreach (var vcamData in _normalOnlyCams)
                    {
                        if (vcamData.Equals(nowVCamData)) continue;

                        vcamData.SetAngleY(angleY);
                    }
                }

                // ロック生存判定
                if (_lockOnTarget != null &&
                    _lockOnTarget.CharaCore != null &&
                    _lockOnTarget.ObjGroupID.SpStatus_IsDead)
                {
                    _lockOnTarget = null;
                }

                // フリーカメラ
                if (CurrentCamData.Value.CamType == CameraData.CameraTypes.FreeCamera)
                {
                    CurrentCamData.Value.FreeCameraUpdate();
                }
            });

        this.FixedUpdateAsObservable()
            .Where(_ => enabled)
            .Subscribe(_ =>
            {
                // 
                _rotateAxisValue_Y *= 0.8f;
            });

        // 
        IGameInputHandler.Instance.OnChangeDevice.Subscribe(device =>
        {
            if (device == IGameInputHandler.DevideTypes.Keyboard)
            {
                CameraMouseMode(true);
            }
            else
            {
                CameraMouseMode(false);
            }
        }).AddTo(this);
        // 
    }

    async void OnEnable()
    {
        await UniTask.DelayFrame(1);
        ChangePlayerCamIndex(_nowNormalCameraIndex, 0);
    }

    public void SetLockOnTarget(SearchTarget target)
    {
        _lockOnTarget = target;
    }

    public void CameraMouseMode(bool enable)
    {
        foreach (var data in _playerVirtualCams)
        {
            data.SetMouseMode(enable);
        }
    }

    public async void SetFreeCameraMode(bool enable, CharacterCore chara)
    {
        if (enable)
        {
            // 現在フリーカメラモード
            if (CurrentCamData.Value.CamType == CameraData.CameraTypes.FreeCamera) return;

            // FreeCameraを検索し、セット
            foreach (var camData in _playerVirtualCams)
            {
                if (camData.CamType == CameraData.CameraTypes.FreeCamera)
                {
                    CurrentCamData.Value = camData;

                    _freeCameraCharacter = chara;
                    if (_freeCameraCharacter != null)
                    {
                        _freeCameraCharacter.FreeCameraMode = true;
                    }

                    _freeCameraModeDisposable.Clear();

                    // 殴られた時
                    chara.OnDamageEvent.Subscribe(param =>
                    {
                        if (param.eveType == Damages.IDamageApplicable.DamageEventTypes.ReceivedDamage)
                        {
                            SetFreeCameraMode(false, null);
                        }
                    }).AddTo(_freeCameraModeDisposable);

                    _freeCameraModeHidePlayerUI = new();
                    _freeCameraModeHidePlayerUI.Enable(null);

                    _freeCameraModeHideWorldUI = new();
                    _freeCameraModeHideWorldUI.Enable(null);

                    _freeCameraModeDisablePlayerOpe = new();
                    _freeCameraModeDisablePlayerOpe.Enable(null);

                    Unity.Cinemachine.CinemachineBrain brain = Unity.Cinemachine.CinemachineBrain.GetActiveBrain(0);
                    brain.IgnoreTimeScale = true;

                    _freeCameraModeTimeScaleNode?.Dispose();
                    _freeCameraModeTimeScaleNode = ITimeScaleManager.Instance.AddTimeScale(0, -1);

                    return;
                }
            }

        }
        else
        {
            await UniTask.DelayFrame(2);

            // Normal Cameraをセット
            CurrentCamData.Value = _normalOnlyCams[_nowNormalCameraIndex];

            if (_freeCameraCharacter != null)
            {
                _freeCameraCharacter.FreeCameraMode = false;
                _freeCameraCharacter.CharaAnimator.SetInteger("DoPerformanceNo", 99);
            }

            _freeCameraModeTimeScaleNode?.Dispose();
            _freeCameraModeTimeScaleNode = null;

            _freeCameraCharacter = null;

            _freeCameraModeHidePlayerUI?.Dispose();
            _freeCameraModeHidePlayerUI = null;

            _freeCameraModeHideWorldUI?.Dispose();
            _freeCameraModeHideWorldUI = null;

            _freeCameraModeDisablePlayerOpe?.Dispose();
            _freeCameraModeDisablePlayerOpe = null;

            Unity.Cinemachine.CinemachineBrain brain = Unity.Cinemachine.CinemachineBrain.GetActiveBrain(0);
            brain.IgnoreTimeScale = false;

            _freeCameraModeTimeScaleNode?.Dispose();
            _freeCameraModeTimeScaleNode = null;
        }
    }

    public async void ChangePlayerCamIndex(int index, float blendDuration)
    {
        if (_stopRotate) return;
        _stopRotate = true;

        _nowNormalCameraIndex = index;

        // Cinemachine Blendの時間を設定
        Unity.Cinemachine.CinemachineBrain brain = Unity.Cinemachine.CinemachineBrain.GetActiveBrain(0);
        brain.DefaultBlend.Time = blendDuration;

        // 現在のカメラデータを記憶
        CurrentCamData.Value = _normalOnlyCams[_nowNormalCameraIndex];

        if (blendDuration > 0)
        {
            await UniTask.Delay((int)(blendDuration * 1000));
        }
        _stopRotate = false;
    }

    public void ChangePlayerCamTypeToNext(bool next, float blendDuration)
    {
        if (CurrentCamData != null && CurrentCamData.Value.CamType == CameraData.CameraTypes.FreeCamera) return;

        if (_normalOnlyCams.Count == 0) return;

        int idx = _nowNormalCameraIndex;

        idx += next ? 1 : -1;
        if (idx < 0) idx = 0;
        if (idx >= _normalOnlyCams.Count) idx = _normalOnlyCams.Count - 1;

        ChangePlayerCamIndex(idx, blendDuration);
    }

    public void ResetRotateAxisToForward(float duration)
    {
        _tweenResetCam.Kill();

        var seq = DOTween.Sequence();
        _tweenResetCam = seq;

        // セット
        _normalOnlyCams[_nowNormalCameraIndex].ResetRotateAxisToForward(duration);
    }

    public void SetRotateAxisByDir(Vector3 dir)
    {
        var forward = dir;
        forward.y = 0;
        forward.Normalize();
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        // セット
        _normalOnlyCams[_nowNormalCameraIndex].SetRotateAxisByDir(dir);
    }

    public void SetRotateAxisY(float value)
    {
        _rotateAxisValue_Y = value;
    }
    public void SetRotateAxisX(float value)
    {
        _rotateAxisValue_X = value;
    }

    // 子GameObjectのVirtualCameraに、targetを設定
    public void SetCameraTarget(Transform target)
    {
        foreach (var vcamData in _playerVirtualCams)
        {
            vcamData.SetTarget(target);
        }
    }

    [System.Serializable]
    public class CameraData : Unity.Cinemachine.AxisState.IInputAxisProvider
    {
        [SerializeField] CameraTypes _camType;
        public CameraTypes CamType => _camType;

        [SerializeField] bool _isTopView;
        public bool IsTopView => _isTopView;

        [SerializeField] protected CinemachineVirtualCamera _vCam;
        public CinemachineVirtualCamera VCam => _vCam;

        Unity.Cinemachine.CinemachineFramingTransposer _transposer;
        Unity.Cinemachine.CinemachinePOV _cinemachinePOV;

        [SerializeField] float _rotatePowerX = 0;
        [SerializeField] float _rotatePowerY = 0;

        public CameraController OwnerCtrl { get; private set; }
        float _origAccelTime = 0.1f;
        float _origDecelTime = 0.1f;

        public enum CameraTypes
        {
            NormalCamera,
            FreeCamera,
        }

        public void Initialize(BattleCameraController owner)
        {
            OwnerCtrl = owner;

            _transposer = _vCam.GetCinemachineComponent<Unity.Cinemachine.CinemachineFramingTransposer>();
            _cinemachinePOV = _vCam.GetCinemachineComponent<Unity.Cinemachine.CinemachinePOV>();

            _origAccelTime = _cinemachinePOV.m_HorizontalAxis.m_AccelTime;
            _origDecelTime = _cinemachinePOV.m_HorizontalAxis.m_DecelTime;
        }

        public void SetInputAxis()
        {
            // 
            _cinemachinePOV.m_HorizontalAxis.SetInputAxisProvider(0, this);
            _cinemachinePOV.m_VerticalAxis.SetInputAxisProvider(1, this);
        }

        public void SetCurrent(bool enable)
        {
            _vCam.Priority = enable ? 10 : -100;
        }

        public float GetAngleY() => _cinemachinePOV.m_HorizontalAxis.Value;
        public void SetAngleY(float angle) => _cinemachinePOV.m_HorizontalAxis.Value = angle;

        public void SetTarget(Transform target)
        {
            _vCam.Follow = target;
            _vCam.LookAt = target;
        }

        public void SetMouseMode(bool enable)
        {
            if (enable)
            {
                _cinemachinePOV.m_HorizontalAxis.m_AccelTime = 0.01f;
                _cinemachinePOV.m_HorizontalAxis.m_DecelTime = 0.01f;
            }
            else
            {
                _cinemachinePOV.m_HorizontalAxis.m_AccelTime = _origAccelTime;
                _cinemachinePOV.m_HorizontalAxis.m_DecelTime = _origDecelTime;
            }
        }

        public void ResetRotateAxisToForward(float duration)
        {
            var forward = _vCam.Follow.forward;
            forward.y = 0;
            forward.Normalize();
            float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

            if (duration > 0)
            {
                var seq = DOTween.Sequence();

                seq.Join(
                    DOVirtual.Float(_cinemachinePOV.m_HorizontalAxis.Value, angle, duration, value =>
                    {
                        _cinemachinePOV.m_HorizontalAxis.Value = value;
                    }).SetEase(Ease.OutCubic)
                );
            }
            else
            {
                _cinemachinePOV.m_HorizontalAxis.Value = angle;
            }
        }

        public void SetRotateAxisByDir(Vector3 dir)
        {
            var forward = dir;
            forward.y = 0;
            forward.Normalize();
            float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

            _cinemachinePOV.m_HorizontalAxis.Value = angle;
        }

        float Unity.Cinemachine.AxisState.IInputAxisProvider.GetAxisValue(int axis)
        {
            if (OwnerCtrl.IsLockingOn && OwnerCtrl.IsFreeCameraMode == false) return 0;
            if (OwnerCtrl._stopRotate) return 0;

            // Y軸
            if (axis == 0)
            {
                return OwnerCtrl._rotateAxisValue_Y * _rotatePowerY;
            }
            // X軸
            if (axis == 1)
            {
                return OwnerCtrl._rotateAxisValue_X * _rotatePowerX;
            }

            return 0;
        }

        // FreeCmaera専用更新処理
        public void FreeCameraUpdate()
        {
            if (OwnerCtrl._freeCameraCharacter == null) return;

            var actions = OwnerCtrl._freeCameraCharacter.InputProvider.GetInputActions(true);

            // 通常モードへ
            if (actions != null &&
                actions.Count >= 1)
            {
                if (actions.Contains(GameInputActionTypes.Menu))
                {
                    OwnerCtrl.SetFreeCameraMode(false, null);
                    return;
                }
            }

            // カメラ操作
            var opeData = OwnerCtrl._freeCameraCharacter.InputProvider.GetFreeCameraModeOperation();

            const float kMaxOffset = 0.4f;

            // X
            _transposer.m_ScreenX -= opeData.axisL.x * 0.01f;
            if (_transposer.m_ScreenX < -kMaxOffset + 0.5f) _transposer.m_ScreenX = -kMaxOffset + 0.5f;
            if (_transposer.m_ScreenX > kMaxOffset + 0.5f) _transposer.m_ScreenX = kMaxOffset + 0.5f;

            // Y
            _transposer.m_ScreenY += opeData.axisL.y * 0.01f;
            if (_transposer.m_ScreenY < -kMaxOffset + 0.5f) _transposer.m_ScreenY = -kMaxOffset + 0.5f;
            if (_transposer.m_ScreenY > kMaxOffset + 0.5f) _transposer.m_ScreenY = kMaxOffset + 0.5f;

            // 回転
            if (SaveData.GameSettings.Instance.CameraRotateMode)
            {
                opeData.axisR.x *= -1;
            }

            OwnerCtrl.SetRotateAxisX(opeData.axisR.y * SaveData.GameSettings.Instance.CameraRotateSpeed);
            OwnerCtrl.SetRotateAxisY(opeData.axisR.x * SaveData.GameSettings.Instance.CameraRotateSpeed);

            // Zoom
            _transposer.m_CameraDistance -= opeData.zoom * 0.05f;
            if (_transposer.m_CameraDistance < 0.5f) _transposer.m_CameraDistance = 0.5f;
            if (_transposer.m_CameraDistance > 3.0f) _transposer.m_CameraDistance = 3.0f;

            // キーボード専用
            if (Keyboard.current != null)
            {
                if (Keyboard.current.digit1Key.isPressed)
                {
                    _vCam.m_Lens.FieldOfView -= 10 * Time.unscaledDeltaTime;
                    if (_vCam.m_Lens.FieldOfView < 10) _vCam.m_Lens.FieldOfView = 10;
                }
                if (Keyboard.current.digit2Key.isPressed)
                {
                    _vCam.m_Lens.FieldOfView += 10 * Time.unscaledDeltaTime;
                    if (_vCam.m_Lens.FieldOfView > 179) _vCam.m_Lens.FieldOfView = 179;
                }
            }

            // 時間進める
            if (actions.Contains(GameInputActionTypes.Guard))
            {
                if (OwnerCtrl._freeCameraModeTimeScaleNode != null)
                {
                    if (OwnerCtrl._freeCameraModeTimeScaleNode._timeScale == 0)
                    {
                        OwnerCtrl._freeCameraModeTimeScaleNode._timeScale = 1.0f;
                    }
                    else
                    {
                        OwnerCtrl._freeCameraModeTimeScaleNode._timeScale = 0.0f;
                    }
                }
            }
        }
    }
*/
}
