using Cysharp.Threading.Tasks;
using UnityEngine;

public class DebugController : MonoBehaviour
{
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
       
    }
}
