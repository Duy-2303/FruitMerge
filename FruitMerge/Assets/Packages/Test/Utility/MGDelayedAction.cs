using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MGDelayedAction : MonoBehaviour
{
    public float delay = 1;
    public UnityEvent action;

    public bool invokeOnEnable = true;
    
    private void OnEnable()
    {
        if (invokeOnEnable) InvokeAction();
    }
    public void InvokeAction()
    {
        StartCoroutine(CorDelay());
    }
    IEnumerator CorDelay()
    {
        if (delay > 0) yield return WaitForSecondCache.Get(delay);
        action.Invoke();
    }
}