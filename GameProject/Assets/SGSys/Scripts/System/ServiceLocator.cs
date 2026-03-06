//========================================================
/// <summary>
/// サービスロケータ
/// </summary>
//========================================================
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ServiceLocator の状態をロード時にリセットするための非ジェネリック初期化クラス
/// </summary>
public static class ServiceLocatorRuntimeReset
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        ServiceLocator.Reset();
    }
}

/// <summary>
/// 非ジェネリック実体（全サービスを一括管理）
/// </summary>
public static class ServiceLocator
{
    static readonly Dictionary<Type, object> s_instances = new();

    public static T Get<T>() where T : class
    {
        return s_instances.TryGetValue(typeof(T), out var obj) ? (T)obj : null;
    }

    public static void Register<T>(T instance) where T : class
    {
        s_instances[typeof(T)] = instance;
    }

    public static void Unregister<T>() where T : class
    {
        s_instances.Remove(typeof(T));
    }

    public static void Reset()
    {
        s_instances.Clear();
    }
}

public static class ServiceLocator<T> where T : class
{
    //サービスの保持・取得
    public static T Instance => ServiceLocator.Get<T>();

    //サービスの登録
    public static void Register(T instance)
    {
        ServiceLocator.Register(instance);
    }
    //サービスの開放
    public static void Unregister()
    {
        ServiceLocator.Unregister<T>();
    }
}

public interface IService<T> where T : class
{
    /// <summary>
    /// インスタンスを取得する(初回の１度目のみServiceLocatorから取得)。
    /// </summary>
    public static T Instance => ServiceLocator<T>.Instance;
}