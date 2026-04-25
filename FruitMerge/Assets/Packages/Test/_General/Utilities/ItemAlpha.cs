using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class ItemAlpha : MonoBehaviour
    {
        [SerializeField] SpriteRenderer[] renderers;
        float[] alphas;
        Color color;

        private float a = -1;

        [OnValueChanged("LateUpdate")]
        public float alpha = 1;

        private void Awake()
        {
            if (alphas == null || alphas.Length != renderers.Length)
            {
                alphas = new float[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                {
                    alphas[i] = renderers[i].color.a <= 0.05f ? 1 : renderers[i].color.a;
                }
            }
        }


        [Button]
        public void SetAlpha(float alpha)
        {
            this.alpha = alpha;
            LateUpdate();
        }

        private void LateUpdate()
        {
            if (a != alpha)
            {
                Awake();

                a = alpha;
                // foreach (var item in renderers)
                // {
                //     item.color = new Color(color.r, item.color.g, item.color.b, a);
                // }
                for(int i = 0; i < renderers.Length; i++)
                {
                    color = renderers[i].color;
                    color.a = alphas[i] * alpha;
                    renderers[i].color = color;
                }
            }
        }

        [Button]
        private void Setup()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>(); 
            enabled = false;
        }

        public void DoAlpha(float alpha, float time, float delay = 0)
        {
            Awake();

            for (int i = 0; i < renderers.Length; i++)
            {
                color = renderers[i].color;
                color.a = alphas[i] * alpha;
                renderers[i].DOColor(color, time).SetDelay(delay);
            }
        }
    }
}