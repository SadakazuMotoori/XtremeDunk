//==================================================================
/// <summary>
/// ゲーム実行時初回から終了まで破棄される事がない常駐シーンクラス
/// 各種システムマネージャは全てここで管理される
/// </summary>
//==================================================================
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using System;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using WindowSystem;

public sealed class PersistentSceneLifecycle : SceneLifecycleBase
{
    // PersistentScene自体のライフサイクル。常駐シーンなので現状は待機処理のみを行う。
    protected override async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
        await UniTask.DelayFrame(1);
    }

    protected override async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {
        await UniTask.DelayFrame(1);
    }

    protected override async UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
    {
        await UniTask.DelayFrame(1);
    }

    protected override async UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
        await UniTask.DelayFrame(1);
    }
}
//==================================================================

public sealed class PersistentSceneLifetimeScope : LifetimeScope
{
    // ゲーム全体で共有するManager Prefabを生成し、各ManagerのServiceLocator登録へつなぐ。
    // PersistentSceneが起動時に生成するMainCamera Prefab。
    // SerializeFieldで参照を持つことで、必須システムをビルドに含める。
     [Header("カメラ管理システム")]
    [SerializeField] GameObject _mainCameraPrefab;
    GameObject _mainCameraInstance;

    // PersistentSceneが起動時に生成するInputManager Prefab。
    // 入力は全シーン共通で必要になるため、MainCameraと同じくPersistentSceneで常駐させる。
    [Header("入力管理システム")]
    [SerializeField] GameObject _inputManagerPrefab;
    GameObject _inputManagerInstance;

    [Header("サウンド管理システム")]
    [SerializeField] GameObject _soundManagerPrefab;
    GameObject _soundManagerInstance;

    // PersistentSceneが起動時に生成するWindowManager Prefab。
    // ウィンドウ制御は全シーン共通で必要になるため、PersistentSceneで常駐させる。
    [Header("ウィンドウ管理システム")]
    [SerializeField] GameObject _windowManagerPrefab;
    GameObject _windowManagerInstance;

    [Header("シーン推移管理システム")]
    [SerializeField] GameObject _sceneTransitionManagerPrefab;
    GameObject _sceneTransitionManagerInstance;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterSceneLifecycle<PersistentSceneLifecycle>();
    }
  
    private void Start()
    {
        // 常駐Managerは、シーン遷移を始める前に必要な順序で初期化する。
        InitializeMainCamera();
        InitializeInputManager();
        InitializeSoundManager();
        InitializeWindowManager();
        InitializeSceneTransitionManager();

        // 製品版ならタイトルシーンへ
        string              _nextSceneName  = "";
#if !IS_PRODUCT
        _nextSceneName = "DebugTopScene";
#else
        _nextSceneName = "TitleScene";
#endif
        if (!SceneManager.GetSceneByName(_nextSceneName).isLoaded)
        {
            ISceneTransitionManager.Instance.RequestSceneChange(_nextSceneName).Forget();
        }
    }

    void InitializeInputManager()
    {
        // すでにInputManagerを生成済みなら、二重生成を避ける。
        if (_inputManagerInstance != null)
        {
            return;
        }

        // 何らかの理由でPlayerInputManagerが先に存在している場合も、重複生成しない。
        if (IPlayerInputManager.Instance != null)
        {
            return;
        }

        if (_inputManagerPrefab == null)
        {
            Debug.LogError("InputManager PrefabがRootSceneLifetimeScopeに設定されていません。");
            return;
        }

        _inputManagerInstance = Instantiate(_inputManagerPrefab, transform);
    }

    void InitializeSoundManager()
    {
        // すでにSoundManagerを生成済みなら、二重生成を避ける。
        if (_soundManagerInstance != null)
        {
            return;
        }

        // 何らかの理由でPlayerInputManagerが先に存在している場合も、重複生成しない。
        if (ISoundManager.Instance != null)
        {
            return;
        }

        if (_soundManagerPrefab == null)
        {
            Debug.LogError("SoundManager PrefabがRootSceneLifetimeScopeに設定されていません。");
            return;
        }

        _soundManagerInstance = Instantiate(_soundManagerPrefab, transform);
    }

    void InitializeMainCamera()
    {
        // すでにMainCameraを生成済みなら、二重生成を避ける。
        if (_mainCameraInstance != null)
        {
            return;
        }

        // 何らかの理由でCameraManagerが先に存在している場合も、重複生成しない。
        if (ICameraManager.Instance != null)
        {
            return;
        }

        if (_mainCameraPrefab == null)
        {
            Debug.LogError("MainCamera PrefabがRootSceneLifetimeScopeに設定されていません。");
            return;
        }

        _mainCameraInstance = Instantiate(_mainCameraPrefab, transform);
    }

    void InitializeWindowManager()
    {
        // すでにWindowManagerを生成済みなら、二重生成を避ける。
        if (_windowManagerInstance != null)
        {
            return;
        }

        // 何らかの理由でWindowManagerが先に存在している場合も、重複生成しない。
        if (IWindowManager.Instance != null)
        {
            return;
        }

        if (_windowManagerPrefab == null)
        {
            Debug.LogError("WindowManager PrefabがRootSceneLifetimeScopeに設定されていません。");
            return;
        }

        _windowManagerInstance = Instantiate(_windowManagerPrefab, transform);
    }

    void InitializeSceneTransitionManager()
    {
        // SceneTransitionManagerも他Managerと同様、既存登録があれば重複生成しない。
        if (_sceneTransitionManagerInstance != null)
        {
            return;
        }

        if (ISceneTransitionManager.Instance != null)
        {
            return;
        }

        if (_sceneTransitionManagerPrefab == null)
        {
            Debug.LogError("SceneTransitionManager PrefabがRootSceneLifetimeScopeに設定されていません。");
            return;
        }

        _sceneTransitionManagerInstance = Instantiate(_sceneTransitionManagerPrefab, transform);
    }
}
