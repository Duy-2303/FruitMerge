using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Link.Cooking
{
    public class EllipseItem2D : MonoBehaviour
    {
        [SerializeField] Transform tf;
        [SerializeField] SpriteRenderer sprite;
        [SerializeField] float timeSpeed = 1f;
        [SerializeField] float rotateSpeed = 0f;
        [SerializeField] bool isGizmos = false;

        Ellipse2D ellipse2D;
        Color color;

        [SerializeField] float xAxis = 0.8f;
        [SerializeField] float yAxis = 0.7f;

        public void OnInit()
        {
            ellipse2D = new Ellipse2D(xAxis, yAxis, tf.localPosition);
        } 

        public void SetTime(float time)
        {
            tf.localPosition = ellipse2D.Evaluate(time * timeSpeed);
        }

        public void SetRotate(float time)
        {
            tf.localRotation = Quaternion.Euler(Vector3.forward * time * rotateSpeed * 360f);
        }

        public void SetLocalScale(float scale)
        {
            tf.localScale = scale * Vector3.one;
        }

        public void SetAlpha(float alpha)
        {
            color = sprite.color;
            color.a = alpha;
            sprite.color = color;
        }

        void OnDrawGizmos()
        {
            if (isGizmos && tf != null)
            {
                new Ellipse2D(xAxis, yAxis, tf.localPosition).OnDrawGizmos(tf.parent.position);
            }
        }

        [Button]
        private void Editor()
        {
            tf = GetComponent<Transform>();
            sprite = GetComponentInChildren<SpriteRenderer>(true);
        }
    }
}
