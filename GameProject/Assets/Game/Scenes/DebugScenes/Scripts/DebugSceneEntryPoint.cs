//==================================================================
/// <summary>
/// 開発版xのみRootシーンから呼び出されるデバッグ用のシーン
/// </summary>
//==================================================================
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement;

// デバッグシーンのSceneEntryPointを実装するコンポーネント
// デバッグシーン用のSceneEntryPoint。シーン開始時に必要な初期化が増えたらここへ追加する。
public sealed class DebugSceneEntoryPoint : SceneEntryPointBase
{
    // デバッグシーンへ入った直後の処理。現状はシーンが1フレーム進むのを待つだけにしている。
    protected override async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {
        await UniTask.DelayFrame(1);
    }

    public void OnButton01(string aa)
    {
        if(UnityEngine.InputSystem.Keyboard.current.aKey.wasPressedThisFrame)
        {

        }
    }
}
