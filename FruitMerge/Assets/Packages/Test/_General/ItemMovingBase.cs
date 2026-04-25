using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link
{
    [RequireComponent(typeof(SortingGroup))]
    public class ItemMovingBase : MonoBehaviour, IItemMoving
    {
        //public enum State { Normal, Pouring, Done }
        //[SerializeField] State state;
        //public override bool IsCanMove => IsState(State.Normal) && base.IsCanMove;

        //public override void ChangeState<T>(T t)
        //{
        //    state = (State)(object)t;
        //}
        //public override bool IsState<T>(T t)
        //{
        //    return state == (State)(object)t;
        //}

        private const int INIT_TARGET_ORDER = -1000;
        public Transform TF => transform;

        protected bool isCanControl = true;
        public virtual bool IsCanMove => isCanControl;

        public bool IsEnable => enabled;
        //public int OrderLayer { get => sprite.sortingOrder; set => sprite.sortingOrder = value; }
        public int OrderLayer
        {
            get => sprite.sortingOrder; 
            set
            {
                //if (this is Link.Cooking.Lobster.SteamDish)
                //{
                //    Debug.Log(value);
                //}
                sprite.sortingOrder = value;
            }
        }

        int targetOrder = INIT_TARGET_ORDER;

        public virtual bool IsDone { get ; protected set; }

        [SerializeField] protected Collider2D collider;
        [SerializeField] protected SortingGroup sprite;
        [SerializeField] protected float delaySaveTime;
        [SerializeField] protected bool isBackWhenDrop = false;

        protected Vector2 savePoint;
        protected Quaternion saveRot;

        protected virtual void Start()
        {
            OnSave(delaySaveTime);
        }

        public virtual void OnBack()
        {
            TF.DOMove(savePoint, 0.3f).OnComplete(CheckOrderLayer);
            TF.DORotateQuaternion(saveRot, 0.3f);
        }

        public virtual void OnClickDown()
        {
            TF.DOKill();
            TF.DOScale(1.2f, 0.1f);
            OrderLayer = 50;
        }
        public virtual void OnClickTake()
        {
            TF.DOScale(1, 0.1f);
            CheckOrderLayer();
        }

        public void CheckOrderLayer()
        {
            if (targetOrder > INIT_TARGET_ORDER)
            {
                OrderLayer = targetOrder;
                targetOrder = INIT_TARGET_ORDER;
            }
            else
            {
                OrderLayer = LevelControl.Ins.GetHighestNoneContactLayer(this, TF.position) + 1;
            }
        }

        public virtual void OnDrop()
        {
            TF.DOScale(1, 0.1f);

            if (isBackWhenDrop)
            {
                OnBack();
            }
            else
            {
                OrderLayer = LevelControl.Ins.GetHighestLayer(this, TF.position) + 1;
                if (IsOutCamera())
                {
                    TF.DOMove((Vector3.zero - TF.position).normalized + TF.position, 0.1f);
                }
            }
        }

        public virtual void OnMove(Vector3 pos, Quaternion rot, float time)
        {
            TF.DOMove(pos, time);
            TF.DORotateQuaternion(rot, time);
        }

        public virtual void OnSavePoint()
        {
            this.savePoint = TF.position;
            this.saveRot = TF.rotation;
        }

        public void OnSave(float delayTime)
        {
            Invoke(nameof(OnSavePoint), delayTime);
        }

        public void OnSave(Vector3 savePoint)
        {
            this.savePoint = savePoint;
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

        public virtual void NextState()
        {}

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
            collider.enabled = false;
        }

        [Button]
        protected virtual void Editor()
        {
            sprite = GetComponent<SortingGroup>();
            if (sprite == null)
            {
                sprite = gameObject.AddComponent<SortingGroup>();
            }

            if (GetComponent<Collider2D>() == null)
            {
                collider = gameObject.AddComponent<BoxCollider2D>();
            }
            else
            {
                collider = GetComponent<Collider2D>();
            }
        }

        //private void OnValidate()
        //{
        //    Editor();
        //}

        protected bool IsOutCamera()
        {
            Vector2 point = Camera.main.WorldToViewportPoint(TF.position);
            return point.x > 0.95f || point.x < 0.05f || point.y > 0.95f || point.y < 0.1f;
        }

        public void SetOrder(int order)
        {
            targetOrder = order;
        }

        public virtual void SetControl(bool isControl)
        {
            isCanControl = isControl;
        }
    }
}