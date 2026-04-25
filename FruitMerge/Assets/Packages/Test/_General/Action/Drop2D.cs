using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class Drop2D : AnimaBase2D
    {
        [SerializeField] Transform[] paths;
        Vector3[] starts;
        [SerializeField] float step = 0.1f, dropTime = 0.2f;

        private void Awake()
        {
            starts =  new Vector3[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                starts[i] = items[i].localPosition;
            }
        }

        [Button]
        public override void OnActive()
        {
            for (int i = 0; i < items.Length; i++)
            {
                Transform item = items[i];
                item.gameObject.SetActive(false);
                Vector3[] path = GetPath(starts[i]);
                item.localPosition = path[0];
                item.DOLocalPath(path, dropTime, PathType.CatmullRom).SetDelay(i * step).OnStart(() => item.gameObject.SetActive(true));
            }

            Invoke(nameof(OnDone), dropTime + items.Length * step);
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