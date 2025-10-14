//==================================================================
/// <summary>
/// �Q�[�����s�����񂩂�I���܂Ŕj������鎖���Ȃ��풓�V�[���N���X
/// �e��V�X�e���}�l�[�W���͑S�Ă����ŊǗ������
/// </summary>
//==================================================================
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using System;
using System.Threading;
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
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterSceneLifecycle<RootSceneLifecycle>();
    }
  
    private void Start()
    {
        // ���i�łȂ�^�C�g���V�[����
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
}

