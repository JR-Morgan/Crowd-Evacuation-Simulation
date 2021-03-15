using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inheriting classes will become singletons.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    #region Singleton
    private static T _instance;
    public static T Instance { get
        {
            if (_instance == null) Debug.LogWarning($"Could not find instance of {typeof(Singleton<T>)}");
            return _instance;
        }
    }

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
