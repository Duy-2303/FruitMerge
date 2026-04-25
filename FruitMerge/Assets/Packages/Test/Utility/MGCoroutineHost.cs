using UnityEngine;
using System.Collections;
using System;

public class MGCoroutineHost : MonoBehaviour
{
    private Action onLateUpdate;

    private static MGCoroutineHost _instance;
    public static MGCoroutineHost instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GetInstance();
                _instance.name = "MGCoroutineHost";
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    public static MGCoroutineHost GetInstance()
    {
        return new GameObject().AddComponent<MGCoroutineHost>();
    }

    /// <summary>
    /// Starts a coroutine, optionally stops a coroutine if not null.
    /// </summary>
    /// <param name="startCor"></param>
    /// <param name="stopCor"></param>
    /// <returns></returns>
    public static Coroutine StartCor(IEnumerator startCor, Coroutine stopCor = null)
    {
        if (stopCor != null) instance.StopCoroutine(stopCor);
        return instance.StartCoroutine(startCor);
    }

    /// <summary>
    /// Stops a coroutine, returns false if coroutine is null.
    /// </summary>
    /// <param name="cor"></param>
    /// <returns></returns>
    public static bool StopCor(Coroutine cor)
    {
        if (cor != null)
        {
            instance.StopCoroutine(cor);
            return true;
        }
        else return false;
    }

    public static void AddToLateUpdate(Action action, bool unique = true)
    {
        if (unique) instance.onLateUpdate -= action;
        instance.onLateUpdate += action;
    }
    public static void RemoveFromLateUpdate(Action action)
    {
        instance.onLateUpdate -= action;
    }

    void LateUpdate()
    {
        if (onLateUpdate != null) onLateUpdate.Invoke();
    }
}
