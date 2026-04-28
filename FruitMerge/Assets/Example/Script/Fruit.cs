using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
namespace DuyDZ.MergeFood.Test
{


    public class Fruit : MonoBehaviour
    {
        public FruitType type;
        public int level;
        [SerializeField ] private Rigidbody2D rb2D;
        [SerializeField] private Collider2D col2D;

        private bool isMerging;
        public bool IsMerging => isMerging;

        public Transform TF => transform;

       

        // =========================
        // SPAWN / DESPAWN
        // =========================

        public void OnSpawn(FruitType type, int level, Vector3 pos)
        {
            this.type = type;
            this.level = level;
            rb2D.mass = 1 + level * 0.5f;
            TF.position = pos;
            TF.rotation = Quaternion.identity;
            TF.localScale = Vector3.one;

            isMerging = false;

            // reset physics
            rb2D.velocity = Vector2.zero;
            rb2D.angularVelocity = 0;
            rb2D.bodyType = RigidbodyType2D.Dynamic;

            gameObject.SetActive(true);

            StartCoroutine(EnableCollisionDelay());
        }

        public void OnDespawn()
        {
            gameObject.SetActive(false);
        }

        IEnumerator EnableCollisionDelay()
        {
            col2D.enabled = false;
            yield return new WaitForSeconds(0.05f);
            col2D.enabled = true;
        }

        // =========================
        // MERGE
        // =========================

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isMerging) return;

            if (collision.gameObject.TryGetComponent(out Fruit other))
            {
                if (other == this) return;
                if (other.IsMerging) return;

                if (other.level == level)
                {
                    MergeManager.Ins.RequestMerge(this, other);
                }
            }
        }

        public void SetMerging()
        {
            isMerging = true;
        }


        [Button]
        public void Add()
        {
            Debug.Log("Odin operated");

            if (GetComponent<Collider2D>() == null)
            {
                col2D = gameObject.AddComponent<CircleCollider2D>();
                Debug.Log("Odin operated");
            }
            if (GetComponent<Rigidbody2D>() == null)
            {
                rb2D = gameObject.AddComponent<Rigidbody2D>();
            }
        }

    }
}