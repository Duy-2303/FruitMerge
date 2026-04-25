using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class ItemIdleBase : MonoBehaviour, IItemIdle
    {

        //public enum State { Dirty, Done }
        //[SerializeField] State state;
        //public override void ChangeState<T>(T t)
        //{
        //    state = (State)(object)t;
        //}
        //public override bool IsState<T>(T t)
        //{
        //    return state == (State)(object)t;
        //}

        public virtual int OrderLayer { get => 0; set => _ = 0; }
        public Transform TF => transform;
        public virtual bool IsDone => false;

        public virtual void OnClickDown()
        {

        }

        public virtual bool OnTake(IItemMoving item)
        {
            return false;
        }
        public virtual void ChangeState<T>(T t) where T : System.Enum
        {

        }
        public virtual bool IsState<T>(T t) where T : System.Enum
        {
            return true;
        }

        public bool IsState<T>(params T[] ts) where T : System.Enum
        {
            foreach (var i in ts)
            {
                if (IsState(i)) return true;
            }
            return false;
        }

        public void IfChangeState<T>(T i, T c) where T : System.Enum
        {
            if (IsState(i)) ChangeState(c);
        }
        public virtual void OnDone()
        {

        }

        [Button]
        protected virtual void Editor()
        {
            if (GetComponent<Collider2D>() == null)
            {
                gameObject.AddComponent<BoxCollider2D>();
            }
        }
        //TF.DOShakePosition(100, 0.01f, 10).SetEase(Ease.Linear).SetDelay(0.5f);

    }
}