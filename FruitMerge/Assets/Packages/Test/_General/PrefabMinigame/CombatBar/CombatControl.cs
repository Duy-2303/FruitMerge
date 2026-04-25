using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking
{
    public class CombatControl : MonoBehaviour
    {
        [SerializeField] Transform vsTF;
        [SerializeField] AnimationCurve curve;
        [SerializeField] Animator anim;
        [SerializeField] Animation vsAnim, attackAnim, defendAnim;
        [SerializeField] Transform barTF;
        [SerializeField] Vector2 vsLimit, barLimit;
        [SerializeField] float rateSpeed = 0.05f, lerpRateSpeed = 10;
        [SerializeField] AudioClip completaClip;

        float rate = 0.5f, targetRate = 0.5f;
        float timeEvaluate, timeDefend;
        bool isActive = false;

        public UnityEvent OnTapEvent, OnDefendEvent;
        public UnityEvent OnDoneEvent;

        [Button]
        public void OnInit()
        {
            rate = targetRate = 0.5f;
            isActive = true;
            gameObject.SetActive(true);
        }

        public void OnDone()
        {
            isActive = false;
            anim.SetTrigger("complete");

            Invoke(nameof(OnComplete), 1.5f);
        }

        private void OnComplete()
        {
            OnDoneEvent?.Invoke();
            gameObject.SetActive(false);
            SoundControl.Ins.PlayFX(completaClip);
        }

        void Update()
        {
            if (isActive && Input.GetMouseButtonDown(0))
            {
                OnClickAttack();
            }

            if (isActive && rate < 1)
            {
                EvaluateDefend();
            }

            rate = Mathf.Lerp(rate, targetRate, Time.deltaTime * lerpRateSpeed);
            SetRate(rate);
        }

        [Button]
        public void OnClickAttack()
        {
            targetRate += rateSpeed;
            if (targetRate >= 1)
            {
                isActive = false;
                OnDone();
            }
            targetRate = Mathf.Clamp(targetRate, 0, 1);
            attackAnim.Stop();
            attackAnim.Play();
            vsAnim.Stop();
            vsAnim.Play();

            OnTapEvent?.Invoke();
        }

        [Button]
        public void OnClickDefend()
        {
            targetRate -= rateSpeed;
            targetRate = Mathf.Clamp(targetRate, 0, 1);
            defendAnim.Stop();
            defendAnim.Play();
            vsAnim.Stop();
            vsAnim.Play();

            OnDefendEvent?.Invoke();
        }

        private void SetRate(float rate)
        {
            vsTF.localPosition = Vector2.Lerp(Vector2.right * vsLimit.x, Vector2.right * vsLimit.y, rate);
            barTF.localPosition = Vector2.Lerp(Vector2.right * barLimit.x, Vector2.right * barLimit.y, rate);
        }

        private void EvaluateDefend()
        {
            timeEvaluate = curve.Evaluate(rate);
            timeDefend += Time.deltaTime;
            if (timeDefend >= timeEvaluate)
            {
                timeDefend = 0;
                OnClickDefend();
            }
        }


    }
}