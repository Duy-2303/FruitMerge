using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking
{
    public class Pan : ItemIdleBase
    {
        public UnityEvent OnDoneEvent;
        [SerializeField] List<Item> items = new List<Item>();
        
        public override bool OnTake(IItemMoving item)
        {
            int index = items[0].index;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].index != index) return false; 

                if(items[i].Equals(item))
                {
                    item.OnDone();
                    items[i].target.SetActive(true);
                    items.RemoveAt(i);

                    if(items.Count == 0)
                    {
                        OnDoneEvent?.Invoke();
                    }
                    return true;
                }
            }
            
            return false;
        }

        [System.Serializable] 
        public class Item 
        {
            [HorizontalGroup("item", 0.5f)]
            public ItemMovingBase item;
            [HorizontalGroup("item", 0.5f)]
            public GameObject target;
            [HorizontalGroup("item", 0.5f)]
            public int index;
        }
    }
}
