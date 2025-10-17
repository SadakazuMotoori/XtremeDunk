using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using R3;
using R3.Triggers;
using UnityEngine;

namespace WindowSystem
{

    /// <summary>
    /// ウィンドウ管理クラス
    /// </summary>
    [DefaultExecutionOrder(-100000)]
    public class WindowManager : MonoBehaviour
    {
        #region Singleton
        public static WindowManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Initialize();
//            DontDestroyOnLoad(gameObject);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void sInit()
        {
            Instance = null;
        }
        #endregion

        [Header("参照")]
        [SerializeField] Transform _popupWindowGroup;

        // 暗幕への参照
        [SerializeField] UnityEngine.UI.Image _imgBlackCurtrain;
        Tween _tweenBlackCurtain;


        NormalWindow _currentNormalWindow = null;

        void Initialize()
        {

        }

        void Start()
        {
            // 
            void ChangeInputMode()
            {
                if (_popupWindowGroup.childCount == 0)
                {
                    // ノーマルウィンドウが存在するなら、それの設定を使用
                    if (_currentNormalWindow != null)
                    {
                        // 文字列がある時だけ
                        if (string.IsNullOrEmpty(_currentNormalWindow.InputActionMap) == false)
                        {
                            PlayerInputManager.Instance.SwitchCurrentActionMap(_currentNormalWindow.InputActionMap);
                        }
                    }
                    else
                    {
                        // デフォルトに戻す
                        PlayerInputManager.Instance.SwitchCurrentActionMap(null);
                    }
                }
                else
                {
                    // 一番上のウィンドウのActionMapにする
                    for (int iChild = _popupWindowGroup.childCount - 1; iChild >= 0; iChild--)
                    {
                        var puWnd = _popupWindowGroup.GetChild(iChild).GetComponent<PopupWindow>();
                        if (puWnd == null) continue;

                        // 文字列がある時だけ
                        if (string.IsNullOrEmpty(puWnd.InputActionMap) == false)
                        {
                            PlayerInputManager.Instance.SwitchCurrentActionMap(puWnd.InputActionMap);
                            return;
                        }
                    }
                }
            }

            //-------------------------------------------------
            // 子ウィンドウの数が変化時、InputActionMapを変更
            //-------------------------------------------------
            _popupWindowGroup.OnTransformChildrenChangedAsObservable()
                .Subscribe(_ =>
                {
                    ChangeInputMode();
                }).AddTo(this);

            //-------------------------------------------------
            // ノーマルウィンドウが変更された時
            //-------------------------------------------------
            Observable.EveryValueChanged(this, x => _currentNormalWindow)
                .Subscribe(_ =>
                {
                    ChangeInputMode();
                });
        }

        // 
        public void SetNormalWindow(NormalWindow window)
        {
            _currentNormalWindow = window;
        }

        /// <summary>
        /// ウィンドウ生成
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <param name="assetAddress">作成するウィンドウアセット</param>
        /// <param name="onInitialize">ウィンドウのOnInitialize前に実行される関数</param>
        /// <returns></returns>
        public async UniTask<TWindow> CreateWindow<TWindow>(object assetAddress, System.Func<TWindow, UniTask> onInitialize) where TWindow : WindowBase
        {
            // アセットロード
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(assetAddress);
            // ロード待ち
            var asset = await handle;
            if(asset == null)
            {
                return null;
            }

            // 親GameObject
            Transform parent = _popupWindowGroup;

            // ウィンドウ作成
            var goWindow = Instantiate(asset, parent);
            var window = goWindow.GetComponent<TWindow>();

            // ウィンドウ破棄される時に、アセットハンドルも解放されるように登録
            goWindow.OnDestroyAsObservable()
                .Subscribe(_ =>
                {
                    UnityEngine.AddressableAssets.Addressables.Release(handle);
                });

            // ウィンドウ初期設定
            await InitWindow(window, onInitialize);

            return window;
        }

        /// <summary>
        /// ウィンドウを初期設定
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        async UniTask InitWindow<TWindow>(TWindow window, System.Func<TWindow, UniTask> onInitialize) where TWindow : WindowBase
        {
            //-----------------------------
            // 非表示状態での初期化処理
            //-----------------------------
            // 非表示にする
            window.gameObject.SetActive(false);

            window.NowState = WindowBase.WindowStates.Initializing;
            // 引数してい外部関数を実行
            if(onInitialize != null)
            {
                await onInitialize(window);
            }
            // ウィンドウの初期化関数を実行
            await window.OnInitialize();
            window.NowState = WindowBase.WindowStates.Initialized;

            //-----------------------------
            // 表示状態での初期化処理
            //-----------------------------
            // 表示
            window.gameObject.SetActive(true);

            window.NowState = WindowBase.WindowStates.Showing;
            // ウィンドウの表示関数を実行
            await window.OnShow();
            window.NowState = WindowBase.WindowStates.Shown;
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public async UniTask CloseWindow(WindowBase window)
        {
            // 既に首領中
            if (window.NowState == WindowBase.WindowStates.Closeing) return;

            // 表示状態まで待つ
            await window.WaitForShown();


            //-----------------------------
            // 破棄時の処理
            //-----------------------------
            window.NowState = WindowBase.WindowStates.Closeing;
            await window.OnClose();

            // 破棄
            Destroy(window.gameObject);
        }
    }

}