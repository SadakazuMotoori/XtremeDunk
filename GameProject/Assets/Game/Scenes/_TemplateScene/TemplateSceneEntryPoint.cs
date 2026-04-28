//==================================================================
/// <summary>
/// 開発版xのみRootシーンから呼び出されるデバッグ用のシーン
/// </summary>
//==================================================================
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;

// デバッグシーンのSceneEntryPointを実装するコンポーネント
public sealed class TemplateSceneEntoryPoint : SceneEntryPointBase
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

    public void OnButton01(string aa)
    {
        if(UnityEngine.InputSystem.Keyboard.current.aKey.wasPressedThisFrame)
        {
        }
    }
}
