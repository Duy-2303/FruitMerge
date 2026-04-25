using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Link.Cooking
{
    public class TestUI_Lock : MonoBehaviour
    {
        [SerializeField] private Image dotImg,lockImg;

        public bool IsLock => lockImg.gameObject.activeSelf;

        public void Select()
        {
            lockImg.color = Color.white;
            transform.DOScale(1.4f, 0.2f).SetEase(Ease.OutBack);
        }

        public void Unselect()
        {
            transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            lockImg.color = Color.gray;
        }

        public void OnLock()
        {
            lockImg.gameObject.SetActive(true);
            dotImg.enabled = false;
            lockImg.color = Color.gray;
        }

        public void OnUnlock()
        {
            lockImg.gameObject.SetActive(false);
            dotImg.enabled = true;
        }

    }
}