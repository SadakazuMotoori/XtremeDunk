//==================================================================
/// <summary>
/// �J���ł̂�Root�V�[������Ăяo�����f�o�b�O�p�̃V�[��
/// </summary>
//==================================================================
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement;

// �f�o�b�O�V�[����SceneEntryPoint����������R���|�[�l���g
public sealed class DebugSceneEntoryPoint : SceneEntryPointBase
{
    protected override async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {
        await UniTask.DelayFrame(1);
    }
}
