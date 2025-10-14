//==================================================================
/// <summary>
/// 開発版のみRootシーンから呼び出されるデバッグ用のシーン
/// </summary>
//==================================================================
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement;

// デバッグシーンのSceneEntryPointを実装するコンポーネント
public sealed class DebugSceneEntoryPoint : SceneEntryPointBase
{
    protected override async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {
        await UniTask.DelayFrame(1);
    }
}
