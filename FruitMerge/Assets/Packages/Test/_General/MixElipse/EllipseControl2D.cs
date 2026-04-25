using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Link.Cooking
{
    public class EllipseControl2D : MonoBehaviour
    {
        [SerializeField] EllipseItem2D[] items;
        [SerializeField] float speed = 1;

        //[Button]
        public void OnInit()
        {
            foreach (var item in items)
            {
                item.OnInit();
            }
        }

        // [OnValueChanged("SetTime")]
        // public float time;

        //[Button]
        public EllipseControl2D SetTime(float time)
        {
            foreach (var item in items)
            {
                item.SetTime(time * speed);
            }
            return this;
        }  
        
        public EllipseControl2D SetRotate(float time)
        {
            foreach (var item in items)
            {
                item.SetRotate(time * speed);
            }
            return this;
        }

        public EllipseControl2D SetScale(float scale)
        {
            foreach (var item in items)
            {
                item.SetLocalScale(scale);
            }
            return this;
        }

        public EllipseControl2D SetAlpha(float alpha)
        {
            foreach (var item in items)
            {
                item.SetAlpha(alpha);
            }
            return this;
        }

        [Button]
        private void Editor()
        {
            items = GetComponentsInChildren<EllipseItem2D>();
        }
    }
}