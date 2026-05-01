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

namespace SGGames.Game.Sys
{
    public sealed class BootSceneEntryPoint : ScopedSceneEntryPoint
    {
        // 起動時に必ず親として扱う常駐シーン名。
        const string kPersistentSceneName = "PersistentScene";

        protected override async UniTask<LifetimeScope> EnsureParentScope(CancellationToken cancellationToken)
        {
            // PersistentSceneが未ロードなら追加ロードし、以降のシーンの親Scopeとして使う。
            // Load persistent scene.
            if (!SceneManager.GetSceneByName(kPersistentSceneName).isLoaded)
            {
                await SceneManager.LoadSceneAsync(kPersistentSceneName, LoadSceneMode.Additive)
                .ToUniTask(cancellationToken: cancellationToken);
            }

            Scene persistentScene = SceneManager.GetSceneByName(kPersistentSceneName);

    #if UNITY_EDITOR
            // Reorder persistent scene.
            EditorSceneManager.MoveSceneBefore(persistentScene, gameObject.scene);
    #endif

            // Build root LifetimeScope container.
            if (persistentScene.TryGetComponentInScene(out LifetimeScope persistentLifetimeScope, true) && persistentLifetimeScope.Container == null)
            {
                await UniTask.RunOnThreadPool(() => persistentLifetimeScope.Build(), cancellationToken: cancellationToken);
            }
            return persistentLifetimeScope;
        }
    }
}