using UnityEngine;

public abstract class Singleton<T> where T : class, new()
{
    private static T _Instance;

    public static T GetInstance()
    {
        if (_Instance == null)
            _Instance = new T();
        return _Instance;
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour
    where T : class, new()
{
    private static T _Instance = null;

    public void Awake()
    {
        _Instance = GetComponent<T>();
    }

    public static T GetInstance()
    {
        return _Instance;
    }
}
