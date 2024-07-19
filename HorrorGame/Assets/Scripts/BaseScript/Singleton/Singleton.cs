using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> where T : Singleton<T>, new()
{
    private static readonly object lockObject = new object();
    protected static T instance = default(T);
    public static bool isExistance { get { return null != instance; } }

    protected Singleton() { Initialize(); }
    ~Singleton() { }
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }

    public void CreateObject()
    { }
    protected virtual void Initialize()
    {}
    protected virtual void Dispose()
    {
        instance = default(T);
        Debug.Log($"{GetType()} isExistance : {isExistance}");
    }
}