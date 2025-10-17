using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using R3;
using R3.Triggers;
using UnityEngine;

namespace WindowSystem
{

    /// <summary>
    /// �E�B���h�E�Ǘ��N���X
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

        [Header("�Q��")]
        [SerializeField] Transform _popupWindowGroup;

        // �Ö��ւ̎Q��
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
                    // �m�[�}���E�B���h�E�����݂���Ȃ�A����̐ݒ���g�p
                    if (_currentNormalWindow != null)
                    {
                        // �����񂪂��鎞����
                        if (string.IsNullOrEmpty(_currentNormalWindow.InputActionMap) == false)
                        {
                            PlayerInputManager.Instance.SwitchCurrentActionMap(_currentNormalWindow.InputActionMap);
                        }
                    }
                    else
                    {
                        // �f�t�H���g�ɖ߂�
                        PlayerInputManager.Instance.SwitchCurrentActionMap(null);
                    }
                }
                else
                {
                    // ��ԏ�̃E�B���h�E��ActionMap�ɂ���
                    for (int iChild = _popupWindowGroup.childCount - 1; iChild >= 0; iChild--)
                    {
                        var puWnd = _popupWindowGroup.GetChild(iChild).GetComponent<PopupWindow>();
                        if (puWnd == null) continue;

                        // �����񂪂��鎞����
                        if (string.IsNullOrEmpty(puWnd.InputActionMap) == false)
                        {
                            PlayerInputManager.Instance.SwitchCurrentActionMap(puWnd.InputActionMap);
                            return;
                        }
                    }
                }
            }

            //-------------------------------------------------
            // �q�E�B���h�E�̐����ω����AInputActionMap��ύX
            //-------------------------------------------------
            _popupWindowGroup.OnTransformChildrenChangedAsObservable()
                .Subscribe(_ =>
                {
                    ChangeInputMode();
                }).AddTo(this);

            //-------------------------------------------------
            // �m�[�}���E�B���h�E���ύX���ꂽ��
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
        /// �E�B���h�E����
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <param name="assetAddress">�쐬����E�B���h�E�A�Z�b�g</param>
        /// <param name="onInitialize">�E�B���h�E��OnInitialize�O�Ɏ��s�����֐�</param>
        /// <returns></returns>
        public async UniTask<TWindow> CreateWindow<TWindow>(object assetAddress, System.Func<TWindow, UniTask> onInitialize) where TWindow : WindowBase
        {
            // �A�Z�b�g���[�h
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(assetAddress);
            // ���[�h�҂�
            var asset = await handle;
            if(asset == null)
            {
                return null;
            }

            // �eGameObject
            Transform parent = _popupWindowGroup;

            // �E�B���h�E�쐬
            var goWindow = Instantiate(asset, parent);
            var window = goWindow.GetComponent<TWindow>();

            // �E�B���h�E�j������鎞�ɁA�A�Z�b�g�n���h������������悤�ɓo�^
            goWindow.OnDestroyAsObservable()
                .Subscribe(_ =>
                {
                    UnityEngine.AddressableAssets.Addressables.Release(handle);
                });

            // �E�B���h�E�����ݒ�
            await InitWindow(window, onInitialize);

            return window;
        }

        /// <summary>
        /// �E�B���h�E�������ݒ�
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        async UniTask InitWindow<TWindow>(TWindow window, System.Func<TWindow, UniTask> onInitialize) where TWindow : WindowBase
        {
            //-----------------------------
            // ��\����Ԃł̏���������
            //-----------------------------
            // ��\���ɂ���
            window.gameObject.SetActive(false);

            window.NowState = WindowBase.WindowStates.Initializing;
            // �������Ă��O���֐������s
            if(onInitialize != null)
            {
                await onInitialize(window);
            }
            // �E�B���h�E�̏������֐������s
            await window.OnInitialize();
            window.NowState = WindowBase.WindowStates.Initialized;

            //-----------------------------
            // �\����Ԃł̏���������
            //-----------------------------
            // �\��
            window.gameObject.SetActive(true);

            window.NowState = WindowBase.WindowStates.Showing;
            // �E�B���h�E�̕\���֐������s
            await window.OnShow();
            window.NowState = WindowBase.WindowStates.Shown;
        }

        /// <summary>
        /// �E�B���h�E�����
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public async UniTask CloseWindow(WindowBase window)
        {
            // ���Ɏ�̒�
            if (window.NowState == WindowBase.WindowStates.Closeing) return;

            // �\����Ԃ܂ő҂�
            await window.WaitForShown();


            //-----------------------------
            // �j�����̏���
            //-----------------------------
            window.NowState = WindowBase.WindowStates.Closeing;
            await window.OnClose();

            // �j��
            Destroy(window.gameObject);
        }
    }

}