using Link;
using Sirenix.OdinInspector;
using UnityEngine;
namespace DuyDZ.MergeFood.Test
{
    public enum FruitType
    {
        Cherry, Blueberry, Lemon, Apple, Grape,
        Peach, Dragon, Pineapple, Coconut, Melon, Watermelon
    }

    public class Fruit : MonoBehaviour
    {
        public FruitType type;
        [SerializeField] protected Animation anims;
        [SerializeField] protected int  level;
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] CircleCollider2D col2D;
        private void Awake()
        {
            
        }
        [Button]
        public void Add()
        {
          anims = GetComponent<Animation>();
            if(anims == null )
            {
                anims = gameObject.AddComponent<Animation>();
            }
          col2D = GetComponent<CircleCollider2D>();
            if(anims == null )
            {
                col2D = gameObject.AddComponent<CircleCollider2D>();
            }
          rb2D = GetComponent<Rigidbody2D>();
            if(anims == null )
            {
                rb2D = gameObject.AddComponent<Rigidbody2D>();
            }
        }
    
    }
}