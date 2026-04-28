using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using R3.Triggers;
using UnityEngine;

namespace WindowSystem
{
    // フェード用Imageに設定する色。現状は黒と白のみを許可している。
    public enum FadeColors
    {
        Black,
        White,
    }

    // FadeInは幕を消す、FadeOutは幕を表示する指定として扱う。
    public enum FadeTypes
    {
        FadeIn,
        FadeOut,
    }

    // フェード幕をどのUIレイヤーの高さに置くかを指定する。
    public enum FadePriorities
    {
        FullScreen,
        UnderLoadingUI,
        UnderHUD,
    }

    public interface IWindowManager : IService<IWindowManager>
    {
        // Window関連システムの公開窓口。利用側はWindowManager本体ではなくこのInterfaceへアクセスする。
        void SetNormalWindow(NormalWindow window);
        UniTask RequestFade(FadeColors fadeColor, FadeTypes fadeType, int durationMilliseconds, FadePriorities priority);
        UniTask<TWindow> CreateWindow<TWindow>(object assetAddress, System.Func<TWindow, UniTask> onInitialize) where TWindow : WindowBase;
        UniTask CloseWindow(WindowBase window);
    }

    /// <summary>
    /// ウィンドウ管理クラス
    /// </summary>
    [DefaultExecutionOrder(-100000)]
    public class WindowManager : MonoBehaviour, IWindowManager
    {
        // PopupWindowの生成・破棄、入力Map切り替え、画面フェードをまとめて管理する。
        #region Singleton
        private static WindowManager Instance => IWindowManager.Instance as WindowManager;

        private void Awake()
        {
            UnityEngine.Object currentManager = IWindowManager.Instance as UnityEngine.Object;
            if (currentManager != null && currentManager != this)
            {
                Destroy(gameObject);
                return;
            }

            ServiceLocator<IWindowManager>.Register(this);
            Initialize();
        }

        private void OnDestroy()
        {
            UnityEngine.Object currentManager = IWindowManager.Instance as UnityEngine.Object;
            if (currentManager == this)
            {
                ServiceLocator<IWindowManager>.Unregister();
            }
        }
        #endregion

        [Header("参照")]
        [SerializeField] Transform _popupWindowGroup;

        // 暗幕への参照
        // フェード幕として使うImage。Prefab側では1x1でも、初期化時に画面サイズへ広げる。
        [SerializeField] UnityEngine.UI.Image _imgFadeCurtrain;
        Tween _tweenFadeCurtain;
        Canvas _canvasFadeCurtain;
        bool _isFadeCurtainInitialized;
        int _fadeCurtainScreenWidth;
        int _fadeCurtainScreenHeight;

        const int kFadeSortingOrderFullScreen = 100;
        const int kFadeSortingOrderUnderLoadingUI = 50;
        const int kFadeSortingOrderUnderHUD = -50;


        NormalWindow _currentNormalWindow = null;

        void Initialize()
        {
            InitializeFadeCurtain();
        }

        void Start()
        {
            // 開いているPopupの有無に応じて、利用するInputActionMapを切り替える。
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
                            IPlayerInputManager.Instance.SwitchCurrentActionMap(_currentNormalWindow.InputActionMap);
                        }
                    }
                    else
                    {
                        // デフォルトに戻す
                        IPlayerInputManager.Instance.SwitchCurrentActionMap(null);
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
                            IPlayerInputManager.Instance.SwitchCurrentActionMap(puWnd.InputActionMap);
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
            // Popupが無い時の入力Map参照先として、現在の通常Windowを保持する。
            _currentNormalWindow = window;
        }

        public async UniTask RequestFade(FadeColors fadeColor, FadeTypes fadeType, int durationMilliseconds, FadePriorities priority)
        {
            // リクエストごとに開始Alphaを決め直し、FadeIn/FadeOutの途中状態に依存しないようにする。
            InitializeFadeCurtain();
            if (_imgFadeCurtrain == null) return;

            UpdateFadeCurtainSize();
            SetFadePriority(priority);

            float targetAlpha = fadeType == FadeTypes.FadeIn ? 0 : 1;
            float startAlpha = fadeType == FadeTypes.FadeIn ? 1 : 0;
            SetFadeColor(fadeColor, startAlpha);

            _imgFadeCurtrain.gameObject.SetActive(true);
            _imgFadeCurtrain.raycastTarget = true;

            _tweenFadeCurtain?.Kill();
            _tweenFadeCurtain = null;

            float duration = Mathf.Max(0, durationMilliseconds) / 1000.0f;
            if (duration <= 0)
            {
                SetFadeColor(fadeColor, targetAlpha);
                ApplyFadeEndState(targetAlpha);
                return;
            }

            Tween tween = _imgFadeCurtrain.DOFade(targetAlpha, duration)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .SetLink(_imgFadeCurtrain.gameObject);
            _tweenFadeCurtain = tween;

            await tween;

            if (_tweenFadeCurtain != tween) return;

            _tweenFadeCurtain = null;
            ApplyFadeEndState(targetAlpha);
        }

        void InitializeFadeCurtain()
        {
            // 初回だけCanvasやサイズを取得し、未使用時はRaycastも表示も無効にしておく。
            if (_isFadeCurtainInitialized) return;
            if (_imgFadeCurtrain == null) return;

            _canvasFadeCurtain = _imgFadeCurtrain.GetComponentInParent<Canvas>();
            UpdateFadeCurtainSize();
            SetFadePriority(FadePriorities.FullScreen);
            SetFadeColor(FadeColors.Black, 0);
            _imgFadeCurtrain.raycastTarget = false;
            _imgFadeCurtrain.gameObject.SetActive(false);
            _isFadeCurtainInitialized = true;
        }

        void UpdateFadeCurtainSize()
        {
            // 解像度が変わった時だけ、1x1のPrefab Imageを現在の画面サイズへ合わせ直す。
            if (_imgFadeCurtrain == null) return;
            if (_fadeCurtainScreenWidth == Screen.width && _fadeCurtainScreenHeight == Screen.height) return;

            var rectTransform = _imgFadeCurtrain.transform as RectTransform;
            if (rectTransform == null) return;

            _fadeCurtainScreenWidth = Screen.width;
            _fadeCurtainScreenHeight = Screen.height;
            rectTransform.sizeDelta = new Vector2(_fadeCurtainScreenWidth, _fadeCurtainScreenHeight);
        }

        void SetFadeColor(FadeColors fadeColor, float alpha)
        {
            Color color = fadeColor == FadeColors.White ? Color.white : Color.black;
            color.a = alpha;
            _imgFadeCurtrain.color = color;
        }

        void SetFadePriority(FadePriorities priority)
        {
            // CanvasのsortingOrderで、フェード幕をLoadingUI/HUDより上または下へ配置する。
            if (_canvasFadeCurtain == null) return;

            switch (priority)
            {
                case FadePriorities.UnderHUD:
                    _canvasFadeCurtain.sortingOrder = kFadeSortingOrderUnderHUD;
                    break;
                case FadePriorities.UnderLoadingUI:
                    _canvasFadeCurtain.sortingOrder = kFadeSortingOrderUnderLoadingUI;
                    break;
                default:
                    _canvasFadeCurtain.sortingOrder = kFadeSortingOrderFullScreen;
                    break;
            }
        }

        void ApplyFadeEndState(float alpha)
        {
            // 完全に透明になった幕は非表示にし、入力を拾わない状態へ戻す。
            bool isVisible = alpha > 0;
            _imgFadeCurtrain.raycastTarget = isVisible;
            _imgFadeCurtrain.gameObject.SetActive(isVisible);
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
