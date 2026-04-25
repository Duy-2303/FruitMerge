using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Link
{
    public class ActionMove : ActionBase
    {
        public enum Direction
        {
            Up, Down, Left, Right
        }

        [SerializeField] Transform tf;
        [SerializeField] public float time = .5f;
        [SerializeField, Range(0, 1)] float rate = 0.9f;
        [SerializeField] Vector2 mid, finish;
        [SerializeField] Direction direction;
        [SerializeField] ItemMovingBase item;

        public override void Active()
        {
            gameObject.SetActive(startActive);
            if (item != null) item.SetControl(false);
            DOVirtual.DelayedCall(delay, () =>
            {
                gameObject.SetActive(true);
                if (Vector2.Distance(mid, Vector2.zero) < 0.05f)
                {
                    tf.DOMove((Vector3)finish + tf.position, time).OnComplete(OnDone);
                }
                else
                {
                    Vector2 start = tf.position;
                    tf.DOMove(start + mid, time * rate).SetEase(Ease.InOutQuart).OnComplete(() => tf.DOMove(start + finish, time * (1 - rate)).OnComplete(OnDone));
                }
            });
        }

        protected override void OnDone()
        {
            base.OnDone();
            if (item != null)
            {
                item.SetControl(true);
                item.OnSavePoint();
            }
            PlayFx();
        }

        private void OnValidate()
        {
            tf = transform;
        }

        protected override void Setup()
        {
            base.Setup();

            switch (state)
            {
                case State.MoveIn:
                    startActive = false;
                    doneActive = true;
                    break;
                case State.MoveOut:
                    startActive = true;
                    doneActive = false;
                    break;
            }

            switch (direction)
            {
                case Direction.Up:
                    switch (state)
                    {
                        case State.MoveIn:
                            mid = new Vector2(0, 4.2f);
                            finish = new Vector2(0, 4);
                            rate = 0.8f;
                            break;
                        case State.MoveOut:
                            rate = 0.2f;
                            mid = new Vector2(0, -.2f);
                            finish = new Vector2(0, 4);
                            break;
                    }
                    break;
                case Direction.Down:
                    switch (state)
                    {
                        case State.MoveIn:
                            mid = new Vector2(0, -4.2f);
                            finish = new Vector2(0, -4);
                            rate = 0.8f;
                            break;
                        case State.MoveOut:
                            mid = new Vector2(0, .2f);
                            finish = new Vector2(0, -4);
                            rate = 0.2f;
                            break;
                    }
                    break;
                case Direction.Left:
                    switch (state)
                    {
                        case State.MoveIn:
                            mid = new Vector2(-4.2f, 0);
                            finish = new Vector2(-4, 0);
                            rate = 0.8f;
                            break;
                        case State.MoveOut:
                            mid = new Vector2(.2f, 0);
                            finish = new Vector2(-4, 0);
                            rate = 0.2f;
                            break;
                    }
                    break;
                case Direction.Right:
                    switch (state)
                    {
                        case State.MoveIn:
                            mid = new Vector2(4.2f, 0);
                            finish = new Vector2(4, 0);
                            rate = 0.8f;
                            break;
                        case State.MoveOut:
                            mid = new Vector2(-.2f, 0);
                            finish = new Vector2(4, 0);
                            rate = 0.2f;
                            break;
                    }
                    break;
            }

            if (item == null) item = GetComponent<ItemMovingBase>();
        }

        [Button]
        private void SetupMoveIn()
        {
            tf.position += (Vector3)finish;
        }

        [Button]
        private void SetupMoveOut()
        {
            tf.position -= (Vector3)finish;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(tf.position, (Vector2)tf.position + mid);
            Gizmos.DrawLine((Vector2)tf.position + mid, (Vector2)tf.position + finish);
            Gizmos.DrawWireSphere((Vector2)tf.position + mid, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere((Vector2)tf.position, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere((Vector2)tf.position + finish, 0.1f);
        }

        public override void Back()
        {
            base.Back();
            tf.position -= (Vector3)finish;
        }
    }
}