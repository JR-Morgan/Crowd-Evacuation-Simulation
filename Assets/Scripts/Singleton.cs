using System;
using UnityEngine;

/// <summary>
/// Inheriting classes will become singletons.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    #region Singleton
    private static T _instance;
    public static T Instance {
        get
        {
            if (!IsSingletonInitialised) throw new UninitialisedSingletonException<T>();
            return _instance;
        }
    }

    public static bool IsSingletonInitialised => _instance != default;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = (T)this;
        }

    }
    #endregion
}

public class UninitialisedSingletonException<T> : Exception where T : Singleton<T>
{

    public UninitialisedSingletonException()
        : base($"Singleton of type {typeof(T)} was accessed before it was initialised." +
            $"Check that an instance of {typeof(T)} was created and that its Awake method was called before accessed ")
    {
    }
}