using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Link
{
    public enum SkakeType { None, Fry, Riped, Overripe, Griller, Heavy }
    public class Shake2D : ActionBase
    {
        [SerializeField] float duration = 100f;
        [SerializeField] Transform square;
        [SerializeField] SkakeType shakeType;

        public override void Active()
        {
            (float strength, int vibrato) = shakeType switch
            {
                SkakeType.Fry => (0.01f, 15),
                SkakeType.Riped => (0.02f, 10),
                SkakeType.Overripe => (0.05f, 10),
                SkakeType.Griller => (0.01f, 10),
                SkakeType.Heavy => (0.3f, 30),
                _ => (0.1f, 10)
            };

            square.DOShakePosition(duration, strength, vibrato).SetEase(Ease.Linear).SetDelay(delay);
        }

        public override void OnStop()
        {
            square.DOKill();
            square.localPosition = Vector3.zero;
        }
        
        protected override void Setup()
        {
            base.Setup();
            if (square == null)
            {
                square = transform.Find("Square");
            }
        }

        public void OnActive(SkakeType shakeType)
        {
            if (this.shakeType == shakeType) return;
            this.shakeType = shakeType;
            Active();
        }


    }
}