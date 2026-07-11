using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class Burst2D : AnimaBase2D
    {
        [SerializeField] float burstTime = 0.2f, rotate = 0f, lerp = 0.3f;

        [Button]
        public override void OnActive()
        {
            for (int i = 0; i < items.Length; i++)
            {
                Transform item = items[i];
                Vector3 point = item.localPosition;
                item.localPosition = Vector3.Lerp(Vector3.zero, item.localPosition, lerp);
                item.DOLocalMove(point, burstTime).OnUpdate(()=> item.Rotate(Vector3.forward * rotate, Space.Self));
            }

            Invoke(nameof(OnDone), burstTime);
        }


    }
}
