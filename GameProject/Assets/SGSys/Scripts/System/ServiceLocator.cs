//========================================================
/// <summary>
/// �T�[�r�X���P�[�^
/// </summary>
//========================================================
using UnityEngine;

public static class ServiceLocator<T> where T : class
{
    //�T�[�r�X�̕ێ��E�擾
    public static T Instance { private set; get; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        Instance = null;
    }

    //�T�[�r�X�̓o�^
    public static void Register(T instance)
    {
        Instance = instance;
    }
    //�T�[�r�X�̊J��
    public static void Unregister()
    {
        Instance = null;
    }
}

public interface IService<T> where T : class
{
    internal static T _instance = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        _instance = null;
    }

    /// <summary>
    /// �C���X�^���X���擾����(����̂P�x�ڂ̂�ServiceLocator����擾)�B
    /// </summary>
    public static T Instance => ServiceLocator<T>.Instance;
}