using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Flip2D : AnimaBase2D
    {
        [SerializeField] Vector3 direct;
        [SerializeField] float time = 0.2f;

        public override void OnActive()
        {
            base.OnActive();

            for (int i = 0; i < items.Length; i++)
            {
                Transform item = items[i];
                Vector3 angle = item.localEulerAngles + direct;
                item.DOLocalRotate(angle, time);
            }
            Invoke(nameof(OnDone), time);
        }
    }
}