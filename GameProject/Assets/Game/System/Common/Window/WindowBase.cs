using UnityEngine;

using R3;
using R3.Triggers;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

namespace WindowSystem
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class WindowBase : MonoBehaviour
    {

        //=============================================
        //
        //=============================================

        // �E�B���h�E�̎��
        public enum WindowStates
        {
            None,           // �������O
            Initializing,   // ��������
            Initialized,    // �������I��
            Showing,        // �\��������
            Shown,          // �\�����ꂽ
            Closeing,       // �I����
        }
        // ���݂̃E�B���h�E�̏������
        public WindowStates NowState { get; set; } = WindowStates.None;

        // �\����Ԃ܂ő҂�
        public async UniTask WaitForShown() => await UniTask.WaitUntil(() => NowState == WindowStates.Shown);

        // �E�B���h�E�����
        public async UniTask CloseWindow()
        {
            await WindowManager.Instance.CloseWindow(this);
        }

        //=============================================
        //
        // �C�x���g�֐�
        //
        //=============================================

        /// <summary>
        /// (��Active)��������
        /// </summary>
        /// <returns></returns>
        public virtual UniTask OnInitialize() => UniTask.CompletedTask;

        /// <summary>
        /// (Active)��������
        /// </summary>
        /// <returns></returns>
        public virtual UniTask OnShow() => UniTask.CompletedTask;

        /// <summary>
        /// �O�̃X�N���[���ɖ߂�A�j������钼�O�Ɏ��s
        /// </summary>
        /// <returns></returns>
        public virtual UniTask OnClose() => UniTask.CompletedTask;


    }
}
