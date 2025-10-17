using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using R3;
using R3.Triggers;
using UnityEngine;

namespace WindowSystem
{
    /// <summary>
    /// ポップアップウィンドウ基本クラス
    /// </summary>
    public abstract class PopupWindow : WindowBase
    {
        // UIのTopのTransform
        [SerializeField] RectTransform _topUITransform;
        public RectTransform TopUITransform => _topUITransform;

        // 選択グループ
        [SerializeField] UISelectableGroup _selectableGroup;
        public UISelectableGroup SelectableGroup => _selectableGroup;

        // 入力マップ名
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

        // 更新処理時に実行される
        public virtual async UniTask OnUpdate()
        {

        }

        // 決定時に実行される
        // 戻り値：false…ウィンドウ閉じる
        public virtual async UniTask<bool> OnDecide(UISelectable selectable)
        {
            return true;
        }

        /// <summary>
        /// 処理
        /// </summary>
        /// <returns>押したボタンのID</returns>
        public async UniTask<(int IDInt, string IDString)> Run()
        {
            var cancelToken = this.GetCancellationTokenOnDestroy();

            // 
            await _selectableGroup.WaitForInitialized();

            // 処理
            while (cancelToken.IsCancellationRequested == false)
            {
                // カーソル処理
                var retCursor = await _selectableGroup.UpdateCursor();

                // 処理実行
                await OnUpdate();

                // 決定
                if (retCursor.action == UISelectable.Actions.Decide)
                {
                    // 決定処理
                    if(await OnDecide(retCursor.select) == false)
                    {
                        // ウィンドウ閉じる
                        await CloseWindow();
                        // 結果を返す
                        return (retCursor.select.IDInt, retCursor.select.IDString);
                    }
                }

                await UniTask.DelayFrame(1);
            }

            return (-1, "");
        }

    }
}
