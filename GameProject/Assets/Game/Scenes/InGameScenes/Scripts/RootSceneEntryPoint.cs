using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement;
using UnityEngine.SceneManagement;

// �^�C�g���V�[����SceneEntryPoint����������R���|�[�l���g
public sealed class RootSceneEntryPoint : SceneEntryPointBase
{
 //   private void Start()
 //   {
    //    SceneManager.UnloadSceneAsync("BootScene");
 //   }
    protected override async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {
        // 1�b�ҋ@
        await Task.Delay(1000); 
    }
}
