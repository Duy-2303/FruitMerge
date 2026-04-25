using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class ItemDrag2D : MonoBehaviour
    {
        public enum ItemCookType { Item_1, Item_2, Item_3, Item_4, Item_5, Item_6, }
        [SerializeField] ItemCookType itemCookType;
        [SerializeField] List<GameObject> items = new List<GameObject>();

        [SerializeField] UnityEvent doneEvent;

        List<ItemDrop2D> drop2Ds = new List<ItemDrop2D>();

        [SerializeField] AudioClip audioClip;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out ItemDrop2D itemDrop2D))
            {
                if (!itemDrop2D.IsActive && itemDrop2D.ItemType == itemCookType && !drop2Ds.Contains(itemDrop2D))
                {
                    drop2Ds.Add(itemDrop2D);
                    itemDrop2D.OnActive();
                    items[drop2Ds.Count - 1].SetActive(false);

                    if (items.Count == drop2Ds.Count)
                    {
                        doneEvent?.Invoke();
                    }

                    if (audioClip != null)
                    {
                        SoundControl.Ins.PlayFX(audioClip);
                    }
                }
            }
        }

        [Button]
        public void Editor()
        {
            items = GetComponentsInChildren<SpriteRenderer>(true).ToList().ConvertAll(x => x.gameObject);
            //need to trigger
        }
    }
}