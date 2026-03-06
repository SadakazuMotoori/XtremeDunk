using UnityEngine;

namespace WindowSystem
{
    public class NormalWindow : MonoBehaviour
    {
        // 入力マップ名
        [SerializeField] string _inputActionMap = "";
        public string InputActionMap => _inputActionMap;


        private void Awake()
        {
            WindowManager.Instance.SetNormalWindow(this);
        }
    }
}