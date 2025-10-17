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

        // ウィンドウの種類
        public enum WindowStates
        {
            None,           // 初期化前
            Initializing,   // 初期化中
            Initialized,    // 初期化終了
            Showing,        // 表示処理中
            Shown,          // 表示された
            Closeing,       // 終了中
        }
        // 現在のウィンドウの処理状態
        public WindowStates NowState { get; set; } = WindowStates.None;

        // 表示状態まで待つ
        public async UniTask WaitForShown() => await UniTask.WaitUntil(() => NowState == WindowStates.Shown);

        // ウィンドウを閉じる
        public async UniTask CloseWindow()
        {
            await WindowManager.Instance.CloseWindow(this);
        }

        //=============================================
        //
        // イベント関数
        //
        //=============================================

        /// <summary>
        /// (非Active)初期処理
        /// </summary>
        /// <returns></returns>
        public virtual UniTask OnInitialize() => UniTask.CompletedTask;

        /// <summary>
        /// (Active)初期処理
        /// </summary>
        /// <returns></returns>
        public virtual UniTask OnShow() => UniTask.CompletedTask;

        /// <summary>
        /// 前のスクリーンに戻り、破棄される直前に実行
        /// </summary>
        /// <returns></returns>
        public virtual UniTask OnClose() => UniTask.CompletedTask;


    }
}
