using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking
{
    public class ItemBack : ItemMovingBase
    {
        [SerializeField] AudioClip clip;
        public override void OnClickDown()
        {
            base.OnClickDown();
            if(clip != null)
            {
                SoundControl.Ins.PlayFX(clip);
            }
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void SetControl(bool isControl)
        {
            base.SetControl(isControl);
        }
    }
}