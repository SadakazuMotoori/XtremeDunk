using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement;
using UnityEngine;
using WindowSystem;

public interface ISceneTransitionManager : IService<ISceneTransitionManager>
{
    // フェード付きシーン遷移の公開窓口。利用側はこのInterface経由で遷移を依頼する。
    bool IsProcessingSceneChange { get; }
    UniTask RequestSceneChange(string sceneName, int fadeOutDurationMilliseconds = 500, int fadeInDurationMilliseconds = 500, FadeColors fadeColor = FadeColors.Black, FadePriorities priority = FadePriorities.FullScreen);
    UniTask RequestSceneChange(ISceneIdentifier sceneIdentifier, int fadeOutDurationMilliseconds = 500, int fadeInDurationMilliseconds = 500, FadeColors fadeColor = FadeColors.Black, FadePriorities priority = FadePriorities.FullScreen);
}

[DefaultExecutionOrder(-100000)]
public class SceneTransitionManager : MonoBehaviour, ISceneTransitionManager
{
    // シーン遷移中の二重リクエストを防ぐための状態フラグ。
    public bool IsProcessingSceneChange { get; private set; }

    void Awake()
    {
        UnityEngine.Object currentManager = ISceneTransitionManager.Instance as UnityEngine.Object;
        if (currentManager != null && currentManager != this)
        {
            Destroy(this);
            return;
        }

        ServiceLocator<ISceneTransitionManager>.Register(this);
    }

    void OnDestroy()
    {
        UnityEngine.Object currentManager = ISceneTransitionManager.Instance as UnityEngine.Object;
        if (currentManager == this)
        {
            ServiceLocator<ISceneTransitionManager>.Unregister();
        }
    }

    public async UniTask RequestSceneChange(string sceneName, int fadeOutDurationMilliseconds = 500, int fadeInDurationMilliseconds = 500, FadeColors fadeColor = FadeColors.Black, FadePriorities priority = FadePriorities.FullScreen)
    {
        // 文字列指定をNavigathena用のSceneIdentifierへ変換して共通処理へ流す。
        if (string.IsNullOrEmpty(sceneName)) return;

        await RequestSceneChange(new BuiltInSceneIdentifier(sceneName), fadeOutDurationMilliseconds, fadeInDurationMilliseconds, fadeColor, priority);
    }

    public async UniTask RequestSceneChange(ISceneIdentifier sceneIdentifier, int fadeOutDurationMilliseconds = 500, int fadeInDurationMilliseconds = 500, FadeColors fadeColor = FadeColors.Black, FadePriorities priority = FadePriorities.FullScreen)
    {
        if (sceneIdentifier == null) return;
        if (IsProcessingSceneChange) return;

        IsProcessingSceneChange = true;

        try
        {
            // FadeOutからFadeInが終わるまで入力を止め、遷移中の誤操作を防ぐ。
            if (IPlayerInputManager.Instance != null)
            {
                IPlayerInputManager.Instance.SetInputBlocked(true);
            }

            if (IWindowManager.Instance != null)
            {
                await IWindowManager.Instance.RequestFade(fadeColor, FadeTypes.FadeOut, fadeOutDurationMilliseconds, priority);
            }

            await GlobalSceneNavigator.Instance.Replace(sceneIdentifier);
            await UniTask.DelayFrame(1);

            // シーン差し替え後、1フレーム待って新しいUIが出揃ってからFadeInを始める。
            if (IWindowManager.Instance != null)
            {
                await IWindowManager.Instance.RequestFade(fadeColor, FadeTypes.FadeIn, fadeInDurationMilliseconds, priority);
            }
        }
        finally
        {
            if (IPlayerInputManager.Instance != null)
            {
                IPlayerInputManager.Instance.SetInputBlocked(false);
            }

            IsProcessingSceneChange = false;
        }
    }
}
