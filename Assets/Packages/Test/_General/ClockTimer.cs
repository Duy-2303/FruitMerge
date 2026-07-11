using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Link
{
    public class ClockTimer : MonoBehaviour
    {
        private Transform _tf;
        public Transform Tf => _tf ? _tf : _tf = transform;
        [SerializeField] private Image timerImage;
        [SerializeField] private float showTime = 0.3f;
        [SerializeField] private float timeOut = 3f;
        [SerializeField] private AudioSource sound;
        
        public UnityEvent _OnTimeStart;
        public UnityEvent _OnTimeOut;
        private Tween _showTween;
        
        private bool _isTimeOut;
        private bool _isStartTimer;
        
        private void Update()
        {
            if (!_isStartTimer) return;
            timerImage.fillAmount -= Time.deltaTime / timeOut;
            if (!(timerImage.fillAmount <= 0)) return;
            _OnTimeOut?.Invoke();
            Hide();
        }
        
        public void Show(float time)
        {
            timeOut = time;
            Show();
        }

        [Sirenix.OdinInspector.Button]
        private void Show()
        {
            if (_isStartTimer) return;
            _OnTimeStart?.Invoke();
            gameObject.SetActive(true);
            Tf.localScale = Vector3.zero;
            timerImage.fillAmount = 1;
            _showTween?.Kill();
            _showTween = Tf.DOScale(Vector3.one, showTime);
            _isStartTimer = true;
            sound?.Play();
        }
        
        private void Hide()
        {
            _showTween?.Kill();
            _showTween = Tf.DOScale(Vector3.zero, showTime)
                .OnComplete(() => gameObject.SetActive(false));
            _isStartTimer = false;
            sound?.Stop();
        }
    }
}