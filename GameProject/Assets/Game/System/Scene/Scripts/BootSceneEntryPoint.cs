//==================================================================
/// <summary>
/// 初回起動時のみ実行されるシーンクラス
/// 常駐するRootSceneを読込み、以降は自動的に破棄される
/// </summary>
//==================================================================
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement.Utilities;
using MackySoft.Navigathena.SceneManagement.VContainer;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public sealed class BootSceneEntryPoint : ScopedSceneEntryPoint
{
    const string kRootSceneName = "RootScene";

    protected override async UniTask<LifetimeScope> EnsureParentScope(CancellationToken cancellationToken)
    {
        // Load root scene.
        if (!SceneManager.GetSceneByName(kRootSceneName).isLoaded)
        {
            await SceneManager.LoadSceneAsync(kRootSceneName, LoadSceneMode.Additive)
              .ToUniTask(cancellationToken: cancellationToken);
        }

        Scene rootScene = SceneManager.GetSceneByName(kRootSceneName);

#if UNITY_EDITOR
        // Reorder root scene.
        EditorSceneManager.MoveSceneBefore(rootScene, gameObject.scene);
#endif

        // Build root LifetimeScope container.
        if (rootScene.TryGetComponentInScene(out LifetimeScope rootLifetimeScope, true) && rootLifetimeScope.Container == null)
        {
            await UniTask.RunOnThreadPool(() => rootLifetimeScope.Build(), cancellationToken: cancellationToken);
        }
        return rootLifetimeScope;
    }
}