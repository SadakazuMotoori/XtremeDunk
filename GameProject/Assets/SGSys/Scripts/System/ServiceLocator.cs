//========================================================
/// <summary>
/// サービスロケータ
/// </summary>
//========================================================
using UnityEngine;

public static class ServiceLocator<T> where T : class
{
    //サービスの保持・取得
    public static T Instance { private set; get; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        Instance = null;
    }

    //サービスの登録
    public static void Register(T instance)
    {
        Instance = instance;
    }
    //サービスの開放
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
    /// インスタンスを取得する(初回の１度目のみServiceLocatorから取得)。
    /// </summary>
    public static T Instance => ServiceLocator<T>.Instance;
}