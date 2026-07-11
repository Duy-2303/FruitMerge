using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Link
{
    public class ItemsChangeAlpha2D : MonoBehaviour
    {
        [SerializeField] SpriteRenderer[] raws;
        [SerializeField] SpriteRenderer[] ripes;

        public void SetAlpha(float alpha)
        {
            // Debug.Log(alpha + " - " + name);
            foreach (var item in raws)
            {
                item.color = new Color(item.color.r, item.color.g, item.color.b, 1 - alpha);
            }
            foreach (var item in ripes)
            {
                item.color = new Color(item.color.r, item.color.g, item.color.b, alpha);
            }
        }

        public void DoAlpha(float alpha, float time)
        {
            foreach (var item in raws)
            {
                item.DOFade(1 - alpha, time);
            }
            foreach (var item in ripes)
            {
                item.DOFade(alpha, time);
            }
        }

        [Button]
        private void SetRaws(string name)
        {
            raws = GetComponentsInChildren<SpriteRenderer>(true);
            List<SpriteRenderer> list = new List<SpriteRenderer>();
            foreach (var item in raws)
            {
                if (item.gameObject.name.Equals(name, StringComparison.Ordinal))
                {
                    list.Add(item);
                }
            }
            raws = list.ToArray();
        }
        [Button]
        private void SetRipes(string name)
        {
            ripes = GetComponentsInChildren<SpriteRenderer>(true);
            List<SpriteRenderer> list = new List<SpriteRenderer>();
            foreach (var item in ripes)
            {
                if (item.gameObject.name.Equals(name, StringComparison.Ordinal))
                {
                    list.Add(item);
                }
            }
            ripes = list.ToArray();
        }

        [Button]
        private void Setup(string name, float alpha = 1)
        {
            ripes = GetComponentsInChildren<SpriteRenderer>();
            foreach (var item in ripes)
            {
                if (item.gameObject.name == name)
                {
                    item.gameObject.SetActive(true);
                    item.color = new Color(item.color.r, item.color.g, item.color.b, alpha);
                }
            }
        }
    }
}