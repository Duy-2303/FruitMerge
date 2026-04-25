using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MGDelayedActionGroup : MonoBehaviour
{
    public bool playOnEnable = true;
    public DelayedAction[] delayedActions;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Play();
        }
    }

    public void Play()
    {
        foreach (DelayedAction act in delayedActions)
        {
            StartCoroutine(CorPlay(act));
        }
    }

    IEnumerator CorPlay(DelayedAction act)
    {
        yield return WaitForSecondCache.Get(act.delay);
        act.Play();
    }

    [Serializable]
    public class DelayedAction
    {
        public float delay;
        public UnityEvent[] actions;

        public void Play()
        {
            foreach (var item in actions)
            {
                item?.Invoke();
            }
        }
    }
}