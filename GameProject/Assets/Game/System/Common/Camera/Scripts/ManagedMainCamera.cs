//==================================================================
/// <summary>
/// �P��̃��C���J�����Ƃ��ē��삳����N���X
/// </summary>
//==================================================================
using UnityEngine;
using UniRx;

public class ManagedMainCamera : MonoBehaviour
{
    [Header("�D�揇��")]
    [SerializeField] int _priority = 0;
    public int Priority => _priority;

    // �J�����ւ̎Q��
    Camera _camera;
    public Camera Cam => _camera;

    // �L��/�������ɁA�J�����}�l�[�W���֓o�^/������X�V�������s��
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
        Debug.Assert(_camera != null, "Camera���A�^�b�`����Ă��Ȃ��I");

        // �J������enabled���Ď����A�X�V������������
        this.ObserveEveryValueChanged(x => _camera.enabled)
            .Subscribe(enabled =>
            {
                ICameraManager.Instance.UpdateManagedMainCameraList();
            });
    }
}
