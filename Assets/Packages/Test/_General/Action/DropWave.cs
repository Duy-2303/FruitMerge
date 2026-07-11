using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class DropWave : AnimaBase2D
    {
        [SerializeField] Transform[] paths;
        [SerializeField] float[] delay;
        [SerializeField] float dropTime = 0.2f, aliveTime = .2f, doneRotate = 25f;

        public override void OnActive()
        {
            for (int i = 0; i < items.Length; i++)
            {
                Transform item = items[i];
                Vector3 offset = item.localPosition - paths.Last().localPosition;
                item.localPosition = offset + paths[0].localPosition;
                item.gameObject.SetActive(false);
                item.DOLocalPath(GetPath(offset), dropTime, PathType.CatmullRom).SetDelay(delay[i]).OnStart(() => item.gameObject.SetActive(true)).OnComplete(() => { item.DOLocalRotate(item.eulerAngles + Vector3.forward * doneRotate, 0.1f); doneEvent?.Invoke(); });
            }

            Invoke(nameof(OnDone), dropTime + aliveTime);
        }

        private Vector3[] GetPath(Vector3 offset)
        {
            Vector3[] vector3s = new Vector3[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                vector3s[i] = paths[i].localPosition + offset;
            }
            return vector3s;
        }

        protected override void Setup()
        {
            base.Setup();

            float minX = 1000, maxX = -1000;
            foreach (var item in items)
            {
                if (minX > item.position.x) minX = item.position.x;
                if (maxX < item.position.x) maxX = item.position.x;
            }

            delay = new float[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                delay[i] = Utilities.GetMapValue(items[i].position.x, minX, maxX, 0, 1) * aliveTime;
            }

            Transform path = transform.Find("Path");
            paths = path.GetChildren();
        }

        [Button]
        private void SetupSameTime()
        {
            base.Setup();

            delay = new float[items.Length];
            for (int i = 1; i < items.Length; i++)
            {
                delay[i] = delay[i - 1] + aliveTime;
            }

            Transform path = transform.Find("Path");
            paths = path.GetChildren();
        }

        private void OnDrawGizmos()
        {
            if (paths.Length <= 1) return;
            for (int i = 1; i < paths.Length; i++)
            {
                Gizmos.DrawLine(paths[i].localPosition + transform.position, paths[i - 1].localPosition + transform.position);
            }
        }

    }
}