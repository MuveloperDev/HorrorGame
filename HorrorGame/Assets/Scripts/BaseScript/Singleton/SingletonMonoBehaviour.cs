using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object lockObject = new object();
    private static T instance = null;

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
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString() + " (Singleton)";
                        DontDestroyOnLoad(singleton);
                    }
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this as T)
        {
            Destroy(this.gameObject);
        }
    }
}