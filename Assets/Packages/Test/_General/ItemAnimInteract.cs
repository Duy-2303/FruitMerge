using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking
{
    public class ItemAnimInteract : ItemIdleBase
    {
        [SerializeField] Animation anim;

        public override void OnClickDown()
        {
            base.OnClickDown();
            anim.Stop();
            anim.Play();
        }

        protected override void Editor()
        {
            base.Editor();
            anim = GetComponent<Animation>();
        }
    }
}