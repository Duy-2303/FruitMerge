using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Link.Cooking
{
    public class ItemDrop2D : MonoBehaviour
    {
        [field:SerializeField] public ItemDrag2D.ItemCookType ItemType { get; private set; }
        [SerializeField] Animation anim;
        [SerializeField] GameObject child;
        [SerializeField] Collider2D col;
        public bool IsActive => child.activeSelf;

        public void OnActive()
        {
            child.SetActive(true);
            anim.Play();
        }

        [Button] 
        public void Setup()
        {
            Editor();
            child.gameObject.SetActive(true);
        }

        [Button]
        public void Editor()
        {
            anim = GetComponent<Animation>();
            col = GetComponent<Collider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
                col.isTrigger = false;
            }
            child = transform.GetChild(0).gameObject;
            child.SetActive(false);
            if(anim == null)
            {
                anim = gameObject.AddComponent<Animation>();
                anim.playAutomatically = false;
            }
        }
    }
}
