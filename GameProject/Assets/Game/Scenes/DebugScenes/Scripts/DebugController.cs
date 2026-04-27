using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

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
        PlayerInputManager inputManager = PlayerInputManager.Instance;
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
