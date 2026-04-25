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

public sealed class RootSceneLifecycle : SceneLifecycleBase
{
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

public sealed class RootSceneLifetimeScope : LifetimeScope
{
    // RootSceneが起動時に生成するMainCamera Prefab。
    // SerializeFieldで参照を持つことで、必須システムをビルドに含める。
     [Header("カメラ管理システム")]
    [SerializeField] GameObject _mainCameraPrefab;
    GameObject _mainCameraInstance;

    // RootSceneが起動時に生成するInputManager Prefab。
    // 入力は全シーン共通で必要になるため、MainCameraと同じくRootSceneで常駐させる。
    [Header("入力管理システム")]
    [SerializeField] GameObject _inputManagerPrefab;
    GameObject _inputManagerInstance;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterSceneLifecycle<RootSceneLifecycle>();
    }
  
    private void Start()
    {
        InitializeInputManager();
        InitializeMainCamera();

        // 製品版ならタイトルシーンへ
        ISceneIdentifier    _newScene       = null;
        string              _nextSceneName  = "";
#if !IS_PRODUCT
        _nextSceneName = "DebugScene";
#else
        _nextSceneName = "TitleScene";
#endif
        if (!SceneManager.GetSceneByName(_nextSceneName).isLoaded)
        {
            _newScene = new BuiltInSceneIdentifier(_nextSceneName);
            GlobalSceneNavigator.Instance.Replace(_newScene);
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
        if (PlayerInputManager.Instance != null)
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
}
