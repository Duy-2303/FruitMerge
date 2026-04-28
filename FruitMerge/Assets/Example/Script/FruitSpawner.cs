using Link;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
namespace DuyDZ.MergeFood.Test
{
    public class FruitSpawner : MonoBehaviour
    {
        public Transform spawnPoint;
        GameObject currentFruit;
        private void Start()
        {
            SpawnNew();
        }
        private void Update()
        {
            if(currentFruit == null) return;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.y =spawnPoint.position.y;
            pos.z = 0;
            currentFruit.transform.position = pos;
            if(Input.GetMouseButtonDown(0))
            {
                Drop();
            }
        }

        private void Drop()
        {
            currentFruit = null;
            Invoke(nameof(SpawnNew), .5f);
        }
        void SpawnNew()
        {
            FruitType type = GetRamdomType();
            string key = type.ToString();

            Vector3 pos = spawnPoint.position;
            int level = (int)type;

            currentFruit = ObjectPooler.current.Spawn(key, pos, type, level);
        }
        FruitType GetRamdomType()
        {
            return (FruitType)UnityEngine.Random.Range(0, 3);
        }
    }
}