using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement;
using UnityEngine;
using WindowSystem;

public interface ISceneTransitionManager : IService<ISceneTransitionManager>
{
    bool IsProcessingSceneChange { get; }
    UniTask RequestSceneChange(string sceneName, int fadeOutDurationMilliseconds = 500, int fadeInDurationMilliseconds = 500, FadeColors fadeColor = FadeColors.Black, FadePriorities priority = FadePriorities.FullScreen);
    UniTask RequestSceneChange(ISceneIdentifier sceneIdentifier, int fadeOutDurationMilliseconds = 500, int fadeInDurationMilliseconds = 500, FadeColors fadeColor = FadeColors.Black, FadePriorities priority = FadePriorities.FullScreen);
}

[DefaultExecutionOrder(-100000)]
public class SceneTransitionManager : MonoBehaviour, ISceneTransitionManager
{
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
