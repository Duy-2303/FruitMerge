using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking
{
    public class TapControl : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private ParticleSystem slashVFX, slashMissVFX;
        [SerializeField] private Transform targetTF, pointTF;
        [SerializeField] private Vector2 targetLimit, pointLimit;
        [SerializeField] private Vector2 pointSpeed;
        [SerializeField] private ItemAlpha itemAlpha;
        [SerializeField] private AudioClip perfectClip, missClip;
        [SerializeField] private int step;
        private float speed;
        bool isCanTap = false;
        public UnityEvent OnTapEvent;
        public UnityEvent OnDoneEvent;

        public void OnInit()
        {
            gameObject.SetActive(true);
        }

        public void OnDone()
        {
            isCanTap = false;
            anim.SetTrigger("complete");
            Invoke(nameof(OnDespawn), 2f);

            SoundControl.Ins.PlayFX(perfectClip);
            SoundControl.Ins.PlayFX(perfectClip, 0.1f);
            SoundControl.Ins.PlayFX(perfectClip, 0.2f);
        }

        public void OnDespawn()
        {
            gameObject.SetActive(false);
            OnDoneEvent?.Invoke();
        }

        [Button]
        public void OnStart()
        {
            isCanTap = true;
            pointTF.localPosition = Vector3.up * pointLimit.y;
            targetTF.localPosition = Vector3.up * Random.Range(targetLimit.x, targetLimit.y);
            speed = Random.Range(pointSpeed.x, pointSpeed.y);
            pointTF.gameObject.SetActive(true);
            itemAlpha.DoAlpha(1, 0.15f);
            //grey layer
        }

        public void OnTap()
        {
            if (!isCanTap) return;
            isCanTap = false;
            // Handle tap event
            pointTF.gameObject.SetActive(false);
            anim.SetTrigger("hit");

            if (IsContact())
            {
                slashVFX.transform.position = pointTF.position;
                slashVFX.Play();

                if (--step <= 0)
                {
                    Invoke(nameof(OnDone), 0.5f);
                }
                else
                {
                    itemAlpha.DoAlpha(0, 0.15f, 0.75f);
                    Invoke(nameof(OnStart), 1f);
                    OnTapEvent?.Invoke();
                }

                SoundControl.Ins.PlayFX(perfectClip);
            }
            else if(Mathf.Abs(pointTF.localPosition.y) < 0.875f)
            {
                slashMissVFX.transform.position = pointTF.position;
                slashMissVFX.Play();
                itemAlpha.DoAlpha(0, 0.15f, 0.75f);
                Invoke(nameof(OnStart), 1f);
                SoundControl.Ins.PlayFX(missClip);
            }else
            {
                //miss
                Invoke(nameof(OnStart), 1f);
                SoundControl.Ins.PlayFX(missClip);
            }

        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTap();
            }

            pointTF.localPosition += speed * Time.deltaTime * Vector3.up;
            if (pointTF.localPosition.y > pointLimit.y)
            {
                pointTF.localPosition = Vector3.up * pointLimit.x;
            }
        }

        private bool IsContact()
        {
            return Vector2.Distance(pointTF.localPosition, targetTF.localPosition) <= 0.12f;
        }


    }
}