using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Link;
using UnityEngine;

namespace Link
{
public class Floating2D : ActionBase
{
        [SerializeField] private float speed = 1;
        [field:SerializeField] public Transform Square { get; private set; }

        [SerializeField] AnimationCurve floatCurve = new AnimationCurve(
             new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(1f, 0.05f, 0f, 0f),
            new Keyframe(3f, -0.05f, 0f, 0f),
            new Keyframe(4f, 0f, 0f, 0f)
        );
        float time = 0f;
        Vector3 pos;

        public override void Active()
        {
            base.Active();
            enabled = true;
            pos = Square.position;
            time = 0;
        }

        public void Active(float delay)
        {
            CancelInvoke(nameof(Active));
            Invoke(nameof(Active), delay);
        }

        public override void OnStop()
        {
            CancelInvoke(nameof(Active));
            base.OnStop();
            enabled = false;
            Square.DOLocalMove(Vector3.zero, 0.1f);
        }

        void LateUpdate()
        {
            time += Time.deltaTime * speed;
            //Debug.Log(floatCurve.Evaluate(time));
            Square.position = pos + floatCurve.Evaluate(time) * Vector3.up;
        }

        protected override void Setup()
        {
            base.Setup();
            enabled = false;
            Square = transform.Find("Square");
            floatCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0.05f),
            new Keyframe(1f, 0.05f, 0f, 0f),
            new Keyframe(3f, -0.05f, 0f, 0f),
            new Keyframe(4f, 0f, 0.00f, -0.05f)
            );
            floatCurve.postWrapMode = WrapMode.Loop;
            floatCurve.preWrapMode = WrapMode.Loop;
        }
    }
}