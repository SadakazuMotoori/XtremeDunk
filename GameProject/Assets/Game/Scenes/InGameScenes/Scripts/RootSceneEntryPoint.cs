using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement;
using UnityEngine.SceneManagement;

// タイトルシーンのSceneEntryPointを実装するコンポーネント
public sealed class RootSceneEntryPoint : SceneEntryPointBase
{
 //   private void Start()
 //   {
    //    SceneManager.UnloadSceneAsync("BootScene");
 //   }
    protected override async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {
        // 1秒待機
        await Task.Delay(1000); 
    }
}
