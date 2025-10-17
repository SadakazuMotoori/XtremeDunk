using System.Collections.Generic;
using R3;
using R3.Triggers;

using UnityEngine;

using UnityEngine.InputSystem;

/// <summary>
/// プレイヤー入力管理
/// </summary>
[DefaultExecutionOrder(-1)]
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

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

        public bool AxisLeft => _axis.WasPerformedThisFrame() ? _axis.ReadValue<Vector2>().x < 0 : false;
        public bool AxisRight => _axis.WasPerformedThisFrame() ? _axis.ReadValue<Vector2>().x > 0 : false;

        public bool AxisUp => _axis.WasPerformedThisFrame() ? _axis.ReadValue<Vector2>().y > 0 : false;
        public bool AxisDown => _axis.WasPerformedThisFrame() ? _axis.ReadValue<Vector2>().y < 0 : false;
        public bool IsPressAxis => _axis.ReadValue<Vector2>().sqrMagnitude > 0;

        public bool Decide => _decide.triggered;
        public bool Cancel => _cancel.triggered;
        public bool Option1 => _option1.triggered;
        public bool Option2 => _option2.triggered;

        public void Initialize(InputActionMap actMap)
        {
            _axis = actMap["Axis"];
            _decide = actMap["Decide"];
            _cancel = actMap["Cancel"];
            _option1 = actMap["Option1"];
            _option2 = actMap["Option2"];
        }

    }
    UIActions _uiAction = new();
    public UIActions UIAction => _uiAction;


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
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 初期設定
        _gameplayAction.Initialize(_playerInput.actions.FindActionMap("Gameplay"));
        _uiAction.Initialize(_playerInput.actions.FindActionMap("UI"));

        //
        _deletectionKeyboard.Enable();
        _deletectionXBOX.Enable();
        _deletectionDS.Enable();
        _deletectionSwitch.Enable();

        // 
    }

    void Update()
    {
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
