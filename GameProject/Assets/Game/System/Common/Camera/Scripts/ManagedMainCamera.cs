//==================================================================
/// <summary>
/// 単一のメインカメラとして動作させるクラス
/// </summary>
//==================================================================
using UnityEngine;
using UniRx;

// 再設計対象
// Cameraコンポーネントが必須なので、付け忘れをUnity側で防ぐ。
[RequireComponent(typeof(Camera))]
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
        // Managerが先に起動している場合だけ、メインカメラ候補として登録する。
        if (ICameraManager.Instance != null)
        {
            ICameraManager.Instance.ManageMainCamera(this);
        }
    }
    private void OnDisable()
    {
        // 無効化されたカメラは、描画対象から外すために登録解除する。
        if (ICameraManager.Instance != null)
        {
            ICameraManager.Instance.UnmanageMainCamera(this);
        }
    }

    void Awake()
    {
        // このクラスは同じGameObject上のCameraをManagerへ渡すための橋渡し役。
        TryGetComponent(out _camera);
        Debug.Assert(_camera != null, "Cameraがアタッチされていない！");

        // カメラのenabledを監視し、更新処理をさせる
        this.ObserveEveryValueChanged(x => _camera.enabled)
            .Subscribe(enabled =>
            {
                // 手動でCamera.enabledを切り替えた場合も、Manager側の現在カメラを更新する。
                if (ICameraManager.Instance != null)
                {
                    ICameraManager.Instance.UpdateManagedMainCameraList();
                }
            });
    }

    void Start()
    {
        // OnEnable時点でManagerがまだ未登録だった場合に備え、Startでも一度だけ登録を試す。
        if (ICameraManager.Instance != null)
        {
            ICameraManager.Instance.ManageMainCamera(this);
        }
    }
}
