using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Link
{
    public class CameraControl : Singleton<CameraControl>
    {
        public enum ShakeType { Light_1, Light_2, Normal_1, Normal_2, Heavy_1, Heavy_2 }
        Transform tf;
        public Transform TF => tf = tf != null ? tf : tf = transform; 
        [SerializeField] Camera camera;
        [SerializeField] float min = 5f, max = 6.25f;

        public float orthographicSize => camera.orthographicSize;
        
        // Start is called before the first frame update
        void Start()
        {
            float target = Utilities.GetMapValue((float)Screen.height/ (float)Screen.width, 1920f/1080f , 1600f/720f, min, max);
            StartCoroutine(IEFixCamera(5, target));
        }

        private IEnumerator IEFixCamera(float start, float target)
        {
            yield return null;
            float time = 0;
            while (time < 0.2f)
            {
                time += Time.deltaTime;
                if (time >= 0.2f) time = 0.2f;
                camera.orthographicSize = Mathf.Lerp(start, target, time / 0.2f);
            }
        }

         [SerializeField] float strength = 0.2f;
         [SerializeField] int vibrato = 10;
         [SerializeField] float randomness = 90f;

        [Button("Shake")]
        public void OnShake(ShakeType shakeType, float duration = 0.5f, float delay = 0)
        {
            switch (shakeType)
            {
                case ShakeType.Light_1:
                    strength = 0.2f;
                    vibrato = 10;
                    randomness = 90f;
                    break;
                case ShakeType.Light_2:
                    strength = 0.5f;
                    vibrato = 15;
                    randomness = 90f;
                    break;
                case ShakeType.Normal_1:
                    strength = 1f;
                    vibrato = 20;
                    randomness = 90f;
                    break;
                case ShakeType.Normal_2:
                    strength = 1.5f;
                    vibrato = 25;
                    randomness = 90f;
                    break;
                case ShakeType.Heavy_1:
                    strength = 2f;
                    vibrato = 30;
                    randomness = 90f;
                    break;
                case ShakeType.Heavy_2:
                    strength = 3f;
                    vibrato = 35;
                    randomness = 90f;
                    break; 
            }

            transform.DOShakePosition(duration, strength, vibrato, randomness, false, true).SetDelay(delay);
        }

        public void OnSize(float target, float duration = 0.5f, float delay = 0)
        {
            camera.DOOrthoSize(target, duration).SetDelay(delay);
        }

        public void OnMove(Vector3 target, float duration = 0.5f, float delay = 0)
        {
            transform.DOMove(target, duration).SetDelay(delay);
        }
    }
}