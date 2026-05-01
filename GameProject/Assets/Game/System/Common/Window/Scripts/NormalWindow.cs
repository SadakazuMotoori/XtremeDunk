using UnityEngine;

namespace SGGames.Game.Sys
{
    public class NormalWindow : MonoBehaviour
    {
        // Popupが開いていない通常画面で使うInputActionMap名。
        // 入力マップ名
        [SerializeField] string _inputActionMap = "";
        public string InputActionMap => _inputActionMap;


        private void Awake()
        {
            // 現在の通常WindowとしてWindowManagerへ登録し、入力Map切り替えの基準にする。
            IWindowManager.Instance.SetNormalWindow(this);
        }
    }
}
