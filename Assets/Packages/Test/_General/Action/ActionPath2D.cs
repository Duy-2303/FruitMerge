using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class ActionPath2D : ActionBase
    {
        [SerializeField] float time = .5f;
        [SerializeField] Transform tf;
        [SerializeField] Ease ease = Ease.Linear;
        [SerializeField] Vector3[] paths;
        public override void Active()
        {
            tf.DOLocalPath(paths, time, PathType.CatmullRom, PathMode.TopDown2D).SetEase(ease).SetDelay(delay).OnComplete(OnDone);
            PlayFx();
        }
        private Vector3[] GetPath(Transform[] paths)
        {
            Vector3[] vector3s = new Vector3[paths.Length];
            for (int i = 0; i < paths.Length ; i++)
            {
                vector3s[i] = paths[i].localPosition + tf.localPosition;
            }
            return vector3s;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(tf.position, paths[0]);

            for (int i = 0; i < paths.Length - 1; i++)
            {
                Gizmos.DrawLine(paths[i], paths[i + 1]);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere((Vector2)tf.position, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(paths.Last(), 0.1f);
        }

        private void OnValidate()
        {
            tf = transform;
        }

        [Button]
        protected override void Setup()
        {
            Transform[] paths = transform.Find("Path").GetChildren();
            this.paths = GetPath(paths);    
        }
    }
}