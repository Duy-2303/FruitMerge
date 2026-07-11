using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    [RequireComponent(typeof(Animation))]
    public class ActionAnim : ActionBase
    {
        [SerializeField] Animation animation;
        [SerializeField] AnimationClip animName;

        public override void Active()
        {
            gameObject.SetActive(startActive);
            DOVirtual.DelayedCall(delay, () =>
            {
                gameObject.SetActive(true);
                animation.Play(animName.name);
                DOVirtual.DelayedCall(animName.length, OnDone);
                PlayFx();
            });
        }

        private void OnValidate()
        {
            animation = GetComponent<Animation>();
            if (animName != null && animation.GetClip(animName.name) != null)
            {
                animation.AddClip(animName, animName.name);
            }
        }

        protected override void Setup()
        {
            base.Setup();
            OnValidate();
            if (GetComponent<ItemAlpha>() == null)
            {
                gameObject.AddComponent<ItemAlpha>();
            }
        }
    }
}