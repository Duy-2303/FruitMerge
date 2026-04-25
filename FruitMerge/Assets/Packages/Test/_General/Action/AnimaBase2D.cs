using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Link
{
    public class AnimaBase2D : MonoBehaviour
    {
        [SerializeField] protected Transform[] items;
        public UnityEvent doneEvent;

        //private void Start()
        //{
        //    Active();
        //}
        [Button]
        public virtual void OnActive()
        {

        }

        protected virtual void OnDone()
        {
            doneEvent?.Invoke();
        }

        [Button]
        protected virtual void Setup()
        {
            if (transform.Find("Path"))
            {
                items = new Transform[transform.childCount - 1];
            }
            else
            {
                items = new Transform[transform.childCount];
            }

            int index = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name != "Path")
                {
                    items[index++] = transform.GetChild(i);
                }
            }
        }

    }
}