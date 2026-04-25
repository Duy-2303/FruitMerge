using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

namespace Link
{
    public abstract class LevelStepBase : MonoBehaviour
    {
        [field: SerializeField] public Sprite Hint { get; private set; }

        [field: SerializeField] public bool IsHaveRetry { get; private set; } = false;
        [SerializeField] protected float delayStartAction = 0;
        [SerializeField] StepType stepType;
        [SerializeField] private float blockTime = 1;
        [SerializeField] private StepData[] stepStart;
        [SerializeField] private StepData[] stepFinish;

        [System.Serializable]
        public class StepData
        {
            [HorizontalGroup()]
            public ActionBase item;
            [HorizontalGroup()]
            public float delay;

            public void Active()
            {
                item.Active();
            }
        }

        [Button]
        public virtual void OnStart()
        {
            DOVirtual.DelayedCall(delayStartAction, () =>
            {
                gameObject.SetActive(true);
                if (stepStart.Length > 0)
                {
                    foreach (var item in stepStart)
                    {
                        item.Active();
                    }
                }
            });

            //LevelControl.Ins.BlockControl(blockTime + delayStartAction);
        }

        public virtual void OnLose()
        {

        }

        [Button]
        public virtual void OnFinish(Action action)
        {
            LevelControl.Ins.BlockControl(1f);
            if (stepFinish.Length > 0)
            {
                foreach (var item in stepFinish)
                {
                    item.Active();
                }
            }
            action?.Invoke();
        }
        public abstract bool IsDone();

        public virtual void AddTrash(IItemMoving item)
        {
        }

        public virtual void RemoveTrash(IItemMoving item)
        {
        }

        public virtual void NextHint()
        {

        }

        [Button]
        public virtual void Setup()
        {

        }

        [Button]
        public virtual void FindStart()
        {
            List<StepData> list = new List<StepData>();
            float saveTime = 1;

            foreach (var item in GameObject.FindObjectsOfType<ActionBase>(true))
            {
                if (item is ActionAnim && item.state == ActionMove.State.MoveIn && item.stepType == stepType)
                {
                    list.Add(new StepData() { item = item, delay = item.delay });
                }
                if (item is ActionMove && item.state == ActionMove.State.MoveIn && item.stepType == stepType)
                {
                    list.Add(new StepData() { item = item, delay = item.delay });
                    saveTime = Mathf.Max(saveTime, (item as ActionMove).delay + (item as ActionMove).time);
                }

                if (item.stepType == StepType.None)
                {
                    Debug.LogError(item.name + " stepType is None " + item.transform.root.name);
                }
                if (item.stepType == StepType.Exception)
                {
                    // Debug.LogError(item.name + " stepType is Break " + item.transform.root.name);
                }
            }
            blockTime = saveTime;
            list.Sort((x, y) => x.item.delay.CompareTo(y.item.delay));
            stepStart = list.ToArray();
        }

        [Button]
        public virtual void FindFinish()
        {
            List<StepData> list = new List<StepData>();

            foreach (var item in GameObject.FindObjectsOfType<ActionBase>(true))
            {
                if (item is ActionAnim && item.state == ActionMove.State.MoveOut && item.stepType == stepType)
                {
                    list.Add(new StepData() { item = item, delay = item.delay });
                }
                if (item is ActionMove && item.state == ActionMove.State.MoveOut && item.stepType == stepType)
                {
                    list.Add(new StepData() { item = item, delay = item.delay });
                }

                if (item.stepType == StepType.None)
                {
                    Debug.LogError(item.name + " stepType is None " + item.transform.root.name);
                }
                if (item.stepType == StepType.Exception)
                {
                    // Debug.LogError(item.name + " stepType is Break "  + item.transform.root.name);
                }
            }

            list.Sort((x, y) => x.item.delay.CompareTo(y.item.delay));
            stepFinish = list.ToArray();
        }

        [Button]
        public void SavedDelayTime()
        {
            foreach (var item in stepStart)
            {
                item.item.delay = item.delay;
            }
            foreach (var item in stepFinish)
            {
                item.item.delay = item.delay;
            }
        }

        [Button]
        public void ResetStart()
        {
            foreach (var item in stepStart)
            {
                item.item.Back();
            }
        }
    }
}
