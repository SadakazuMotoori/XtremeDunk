using System.Collections.Generic;
using R3;
using R3.Triggers;

using UnityEngine;

using UnityEngine.InputSystem;

/// <summary>
/// プレイヤー入力サービスの公開窓口。
/// 呼び出し側は具体クラスではなく IPlayerInputManager.Instance を使うことで、
/// CameraManager と同じServiceLocator経由で現在の入力管理インスタンスへアクセスできる。
/// </summary>
public interface IPlayerInputManager : IService<IPlayerInputManager>
{
    /// <summary>
    /// UI操作用の入力状態を取得する。
    /// UISelectableなど、UI上の選択・決定・キャンセル処理はここから入力を読む。
    /// </summary>
    PlayerInputManager.UIActions UIAction { get; }

    /// <summary>
    /// 直近の入力デバイスがキーボード/マウス系かどうかを取得する。
    /// 表示する操作説明やカーソル表示の切り替えで利用する。
    /// </summary>
    bool IsNowKeyboardMouseMode { get; }

    /// <summary>
    /// 入力デバイスが切り替わった時に通知されるObservable。
    /// UI表示や操作説明を、最後に使われたデバイスに合わせたい時に購読する。
    /// </summary>
    Observable<PlayerInputManager.DevideTypes> OnChangeDevice { get; }

    bool IsInputBlocked { get; }
    void SetInputBlocked(bool isBlocked);

    /// <summary>
    /// 現在有効なInputActionMapを切り替える。
    /// 画面状態に応じて、Gameplay用・UI用などの入力受付を切り替えるために使う。
    /// </summary>
    void SwitchCurrentActionMap(string mapName);
}

/// <summary>
/// プレイヤー入力管理
/// </summary>
[RequireComponent(typeof(PlayerInput))]
[DefaultExecutionOrder(-1)]
public class PlayerInputManager : MonoBehaviour, IPlayerInputManager
{
    // 既存コードがPlayerInputManager.Instanceを参照しているため、ServiceLocator経由の互換入口として残す。
    private static PlayerInputManager Instance => IPlayerInputManager.Instance as PlayerInputManager;

    [SerializeField] PlayerInput _playerInput;

    //======================================
    // Gameplay
    //======================================
    public class GameplayActions
    {

        public void Initialize(InputActionMap actMap)
        {

        }
    }
    GameplayActions _gameplayAction = new();

    //======================================
    // UI
    //======================================
    public class UIActions
    {
        InputAction _axis { get; set; }
        InputAction _decide { get; set; }
        InputAction _cancel { get; set; }
        InputAction _option1 { get; set; }
        InputAction _option2 { get; set; }
        bool _isInputBlocked;

        public bool AxisLeft => _isInputBlocked == false && _axis != null && _axis.WasPerformedThisFrame() ? _axis.ReadValue<Vector2>().x < 0 : false;
        public bool AxisRight => _isInputBlocked == false && _axis != null && _axis.WasPerformedThisFrame() ? _axis.ReadValue<Vector2>().x > 0 : false;

        public bool AxisUp => _isInputBlocked == false && _axis != null && _axis.WasPerformedThisFrame() ? _axis.ReadValue<Vector2>().y > 0 : false;
        public bool AxisDown => _isInputBlocked == false && _axis != null && _axis.WasPerformedThisFrame() ? _axis.ReadValue<Vector2>().y < 0 : false;
        public bool IsPressAxis => _isInputBlocked == false && _axis != null && _axis.ReadValue<Vector2>().sqrMagnitude > 0;

        public bool Decide => _isInputBlocked == false && _decide != null && _decide.triggered;
        public bool Cancel => _isInputBlocked == false && _cancel != null && _cancel.triggered;
        public bool Option1 => _isInputBlocked == false && _option1 != null && _option1.triggered;
        public bool Option2 => _isInputBlocked == false && _option2 != null && _option2.triggered;

        public void SetInputBlocked(bool isBlocked)
        {
            _isInputBlocked = isBlocked;
        }

        public void Initialize(InputActionMap actMap)
        {
            // ActionMapが未設定でも、UI側がnull参照で止まらないようにする。
            if (actMap == null)
            {
                return;
            }

            _axis = actMap.FindAction("Axis", false);
            _decide = actMap.FindAction("Decide", false);
            _cancel = actMap.FindAction("Cancel", false);
            _option1 = actMap.FindAction("Option1", false);
            _option2 = actMap.FindAction("Option2", false);
        }

    }
    UIActions _uiAction = new();
    public UIActions UIAction => _uiAction;

    bool _isInputBlocked;
    public bool IsInputBlocked => _isInputBlocked;
    public void SetInputBlocked(bool isBlocked)
    {
        _isInputBlocked = isBlocked;
        _uiAction.SetInputBlocked(isBlocked);
    }


    //======================================
    // 入力デバイスの種類
    //======================================
    public enum DevideTypes
    {
        None,
        Keyboard,
        XBOX,
        PlayStation,
        Switch,

        _Count_,
    }
    DevideTypes _lastInputDevice = DevideTypes.None;

    // コントローラ識別用
    private InputAction _deletectionKeyboard = new InputAction(type: InputActionType.PassThrough, binding: "<Keyboard>/AnyKey", interactions: "Press");
    private InputAction _deletectionXBOX = new InputAction(type: InputActionType.PassThrough, binding: "<XInputController>/*", interactions: "Press");
    private InputAction _deletectionDS = new InputAction(type: InputActionType.PassThrough, binding: "<DualShockGamepad>/*", interactions: "Press");
    private InputAction _deletectionSwitch = new InputAction(type: InputActionType.PassThrough, binding: "<SwitchProControllerHID>/*", interactions: "Press");

    // 現在はキーマウ？
    public bool IsNowKeyboardMouseMode => _lastInputDevice == DevideTypes.Keyboard;


    //======================================
    //
    // イベント
    //
    //======================================

    // 入力デバイスが変更された時
    BehaviorSubject<DevideTypes> _onChangeDevice = new(DevideTypes.None);
    public Observable<DevideTypes> OnChangeDevice => _onChangeDevice;



    //======================================
    // 
    private void Awake()
    {
        // PlayerInputManagerは全体で1つだけ使う。重複した場合は、先にServiceLocatorへ登録済みのものを優先する。
        // ServiceLocatorはinterfaceで返すため、Unity独自のnull判定を使えるようObjectとして比較する。
        UnityEngine.Object currentManager = IPlayerInputManager.Instance as UnityEngine.Object;
        if (currentManager != null && currentManager != this)
        {
            Destroy(gameObject);
            return;
        }

        // 他のクラスは IPlayerInputManager.Instance から、このManagerへアクセスする。
        // 既存の PlayerInputManager.Instance も、この登録内容を参照する。
        ServiceLocator<IPlayerInputManager>.Register(this);

        // Prefab側の設定漏れがあっても、同じGameObject上のPlayerInputを自動取得する。
        if (_playerInput == null)
        {
            TryGetComponent(out _playerInput);
        }

        if (_playerInput == null || _playerInput.actions == null)
        {
            Debug.LogError("PlayerInputManagerにPlayerInput、またはInputActionが設定されていません。");
            enabled = false;
            return;
        }

        // 初期設定
        _gameplayAction.Initialize(_playerInput.actions.FindActionMap("Gameplay", false));
        _uiAction.Initialize(_playerInput.actions.FindActionMap("UI", false));

        //
        _deletectionKeyboard.Enable();
        _deletectionXBOX.Enable();
        _deletectionDS.Enable();
        _deletectionSwitch.Enable();

        // 
    }

    private void OnDestroy()
    {
        // 自分が登録したサービスだけを解除し、別のPlayerInputManagerの登録を消さないようにする。
        // interfaceのまま比較するとUnityのObject比較にならないため、Objectへ戻してから比較する。
        UnityEngine.Object currentManager = IPlayerInputManager.Instance as UnityEngine.Object;
        if (currentManager == this)
        {
            ServiceLocator<IPlayerInputManager>.Unregister();
        }

        // コードで作成したInputActionは、破棄時に明示的に解放する。
        _deletectionKeyboard.Dispose();
        _deletectionXBOX.Dispose();
        _deletectionDS.Dispose();
        _deletectionSwitch.Dispose();
        _onChangeDevice.Dispose();
    }

    void Update()
    {
        if (_isInputBlocked)
        {
            return;
        }

        // 入力デバイスの判定
        if (_deletectionKeyboard.triggered || (Mouse.current != null && Mouse.current.delta.magnitude > 0))
        {
            if (_lastInputDevice != DevideTypes.Keyboard)
            {
                // マウスアンロック中の場合は、カーソル表示
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.visible = true;
                }

                Debug.Log("[Inputデバイス変更]キーボード");
                _onChangeDevice.OnNext(DevideTypes.Keyboard);
                _lastInputDevice = DevideTypes.Keyboard;
            }
        }

        // XBOXコントローラー
        else if (_deletectionXBOX.triggered)
        {
            if (_lastInputDevice != DevideTypes.XBOX)
            {
                // マウスアンロック中の場合は、カーソル非表示
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.visible = false;
                }

                Debug.Log("[Inputデバイス変更]XBOXコントローラ");
                _onChangeDevice.OnNext(DevideTypes.XBOX);
                _lastInputDevice = DevideTypes.XBOX;
            }
        }
        // PlayStationコントローラー
        else if (_deletectionDS.triggered)
        {
            if (_lastInputDevice != DevideTypes.PlayStation)
            {
                // マウスアンロック中の場合は、カーソル非表示
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.visible = false;
                }

                Debug.Log("[Inputデバイス変更]デュアルショック");
                _onChangeDevice.OnNext(DevideTypes.PlayStation);
                _lastInputDevice = DevideTypes.PlayStation;
            }
        }
        // Switchコントローラー
        else if (_deletectionSwitch.triggered)
        {
            if (_lastInputDevice != DevideTypes.Switch)
            {
                // マウスアンロック中の場合は、カーソル非表示
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.visible = false;
                }

                Debug.Log("[Inputデバイス変更]Switchコントローラ");
                _onChangeDevice.OnNext(DevideTypes.Switch);
                _lastInputDevice = DevideTypes.Switch;
            }
        }
    }

    /// <summary>
    /// ActionMapを変更
    /// </summary>
    public void SwitchCurrentActionMap(string mapName)
    {
        if (_playerInput == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(mapName) == false)
        {
            _playerInput.SwitchCurrentActionMap(mapName);
            Debug.Log($"[PlayerInputManager] ****** SwitchCurrentActionMap : {mapName}");
        }
        else
        {
            _playerInput.SwitchCurrentActionMap(_playerInput.defaultActionMap);
            Debug.Log($"[PlayerInputManager] ****** SwitchCurrentActionMap : {_playerInput.defaultActionMap}");
        }
    }

}
