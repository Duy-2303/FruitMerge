using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Plays a random action in this group.
/// </summary>
public class MGRandomAction : MonoBehaviour
{
    public bool playOnEnable = true;
    public UnityEvent[] actions;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Play();
        }
    }

    public void Play()
    {
        actions.GetRandomElement().Invoke();
    }

}