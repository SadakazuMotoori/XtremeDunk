using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using WindowSystem;

public class DebugController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _item1Text;
    [SerializeField] TextMeshProUGUI _item2Text;

    int _selectedIndex;
    TextMeshProUGUI[] _itemTexts;

    private void Awake()
    {
        Debug.Log("[Window] Awake");
    }

    void Start()
    {
        IWindowManager.Instance.RequestFade(FadeColors.Black, FadeTypes.FadeIn, 500, FadePriorities.FullScreen);

        Run();
    }

    async void Run()
    {
        _itemTexts = new[] { _item1Text, _item2Text };
        RefreshSelection();
        await UniTask.CompletedTask;
    }

    private void Update()
    {
        IPlayerInputManager inputManager = IPlayerInputManager.Instance;
        if (_itemTexts == null || inputManager == null)
        {
            return;
        }

        PlayerInputManager.UIActions inputUI = inputManager.UIAction;
        if (inputUI.AxisUp)
        {
            _selectedIndex = _selectedIndex == 0 ? 1 : 0;
            RefreshSelection();
        }
        else if (inputUI.AxisDown)
        {
            _selectedIndex = _selectedIndex == 0 ? 1 : 0;
            RefreshSelection();
        }
    }

    void RefreshSelection()
    {
        for (int i = 0; i < _itemTexts.Length; i++)
        {
            if (_itemTexts[i] == null)
            {
                continue;
            }

            _itemTexts[i].color = i == _selectedIndex ? Color.red : Color.white;
        }
    }
}
