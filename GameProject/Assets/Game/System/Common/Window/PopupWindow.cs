using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using R3;
using R3.Triggers;
using UnityEngine;

namespace WindowSystem
{
    /// <summary>
    /// �|�b�v�A�b�v�E�B���h�E��{�N���X
    /// </summary>
    public abstract class PopupWindow : WindowBase
    {
        // UI��Top��Transform
        [SerializeField] RectTransform _topUITransform;
        public RectTransform TopUITransform => _topUITransform;

        // �I���O���[�v
        [SerializeField] UISelectableGroup _selectableGroup;
        public UISelectableGroup SelectableGroup => _selectableGroup;

        // ���̓}�b�v��
        [SerializeField] string _inputActionMap = "UI";
        public string InputActionMap => _inputActionMap;

        // 
        public PopupWindow PrevWindow { get; set; }

        // 
        public async UniTask CloseTopOwnerWindow()
        {
            var wnd = PrevWindow;
            while(wnd != null)
            {
                await wnd.CloseWindow();

                wnd = wnd.PrevWindow;
            }
        }

        public override async UniTask OnInitialize()
        {
            await base.OnInitialize();

        }

        public override async UniTask OnShow()
        {
            await TopUITransform.DOLocalMoveY(500, 0.1f).From();
        }

        public override async UniTask OnClose()
        {
            await TopUITransform.DOLocalMoveY(-500, 0.1f).SetRelative();
        }

        // �X�V�������Ɏ��s�����
        public virtual async UniTask OnUpdate()
        {

        }

        // ���莞�Ɏ��s�����
        // �߂�l�Ffalse�c�E�B���h�E����
        public virtual async UniTask<bool> OnDecide(UISelectable selectable)
        {
            return true;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <returns>�������{�^����ID</returns>
        public async UniTask<(int IDInt, string IDString)> Run()
        {
            var cancelToken = this.GetCancellationTokenOnDestroy();

            // 
            await _selectableGroup.WaitForInitialized();

            // ����
            while (cancelToken.IsCancellationRequested == false)
            {
                // �J�[�\������
                var retCursor = await _selectableGroup.UpdateCursor();

                // �������s
                await OnUpdate();

                // ����
                if (retCursor.action == UISelectable.Actions.Decide)
                {
                    // ���菈��
                    if(await OnDecide(retCursor.select) == false)
                    {
                        // �E�B���h�E����
                        await CloseWindow();
                        // ���ʂ�Ԃ�
                        return (retCursor.select.IDInt, retCursor.select.IDString);
                    }
                }

                await UniTask.DelayFrame(1);
            }

            return (-1, "");
        }

    }
}
