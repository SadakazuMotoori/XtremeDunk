//==================================================================
/// <summary>
/// 単一のメインカメラとして動作させるクラス
/// </summary>
//==================================================================
using UnityEngine;
using UniRx;

public class ManagedMainCamera : MonoBehaviour
{
    [Header("優先順位")]
    [SerializeField] int _priority = 0;
    public int Priority => _priority;

    // カメラへの参照
    Camera _camera;
    public Camera Cam => _camera;

    // 有効/無効時に、カメラマネージャへ登録/解除や更新処理を行う
    private void OnEnable()
    {
        ICameraManager.Instance.ManageMainCamera(this);
    }
    private void OnDisable()
    {
        ICameraManager.Instance.UnmanageMainCamera(this);
    }

    void Awake()
    {
        TryGetComponent(out _camera);
        Debug.Assert(_camera != null, "Cameraがアタッチされていない！");

        // カメラのenabledを監視し、更新処理をさせる
        this.ObserveEveryValueChanged(x => _camera.enabled)
            .Subscribe(enabled =>
            {
                ICameraManager.Instance.UpdateManagedMainCameraList();
            });
    }
}
