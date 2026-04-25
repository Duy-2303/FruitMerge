using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Link
{
    public enum StepType
    {
        None, Step_0, Step_1, Step_2, Step_3, Step_4, Step_5, Step_6, Step_7, Step_8, Step_9, Exception,
    }

    public abstract class ActionBase : MonoBehaviour
    {
        public enum State
        {
            MoveIn, MoveOut, None
        }
        [field: SerializeField] public State state {get; private set;}
        public UnityEvent doneEvent;
        [SerializeField] protected bool startActive = true, doneActive = true;
        [SerializeField] public float delay;
        [field: SerializeField] public StepType stepType {get; set;}
        [SerializeField] protected AudioClip clip;

        [Button]
        public virtual void Active(){}

        public virtual void OnStop(){}

        protected virtual void OnDone()
        {
            gameObject.SetActive(doneActive);
            doneEvent?.Invoke();
        }

        [Button]
        protected virtual void Setup()
        {

        }

        protected void PlayFx()
        {
            if (clip != null)
            {
                SoundControl.Ins.PlayFX(clip);
            }
        }

        public virtual void Back()
        {
            gameObject.SetActive(startActive);
        }
    }
}