using Link;
using UnityEngine;

namespace DuyDZ.MergeFood.Test
{
    public class FruitSpawner : MonoBehaviour
    {
        public Transform spawnPoint;

        private GameObject currentFruit;

        private bool isDragging;

        private bool canSpawn = true;

        private void Update()
        {
            // CLICK -> SPAWN
            if (Input.GetMouseButtonDown(0) && canSpawn)
            {
                SpawnNew();

                isDragging = true;
            }

            // DRAG
            if (Input.GetMouseButton(0)
                && isDragging
                && currentFruit != null)
            {
                Vector3 pos =
                    Camera.main.ScreenToWorldPoint(Input.mousePosition);

                pos.y = spawnPoint.position.y;
                pos.z = 0;

                currentFruit.transform.position = pos;
            }

            // DROP
            if (Input.GetMouseButtonUp(0)
                && isDragging
                && currentFruit != null)
            {
                isDragging = false;

                Drop();
            }
        }

        private void Drop()
        {
            Rigidbody2D rb =
                currentFruit.GetComponent<Rigidbody2D>();

            rb.bodyType = RigidbodyType2D.Dynamic;

            currentFruit = null;

            canSpawn = false;

            Invoke(nameof(ResetSpawn), 0.5f);
        }

        private void ResetSpawn()
        {
            canSpawn = true;
        }

        private void SpawnNew()
        {
            FruitType type = GetRandomType();

            string key = type.ToString();

            currentFruit =
                ObjectPooler.current.Spawn(
                    key,
                    spawnPoint.position,
                    type,
                    (int)type);

            Rigidbody2D rb =
                currentFruit.GetComponent<Rigidbody2D>();

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;

            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private FruitType GetRandomType()
        {
            return (FruitType)Random.Range(0, 3);
        }
    }
}