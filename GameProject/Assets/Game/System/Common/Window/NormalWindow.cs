using UnityEngine;

namespace WindowSystem
{
    public class NormalWindow : MonoBehaviour
    {
        // “ü—Íƒ}ƒbƒv–¼
        [SerializeField] string _inputActionMap = "";
        public string InputActionMap => _inputActionMap;


        private void Awake()
        {
            WindowManager.Instance.SetNormalWindow(this);
        }
    }
}