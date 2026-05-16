using UnityEngine;
using System.Collections.Generic;
using DuyDZ.MergeFood.Test;

namespace DuyDZ.MergeFood
{
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler current;

        [System.Serializable]
        public class Pool
        {
            public string key;          // "Cherry", "Apple", ...
            public GameObject prefab;
            public int amount = 10;
        }

        public List<Pool> pools;

        private Dictionary<string, List<GameObject>> poolDict;
        private Dictionary<string, GameObject> prefabDict;

        private void Awake()
        {
            current = this;
            poolDict = new Dictionary<string, List<GameObject>>();
            prefabDict = new Dictionary<string, GameObject>();

            foreach (var pool in pools)
            {
                List<GameObject> list = new List<GameObject>();

                for (int i = 0; i < pool.amount; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    list.Add(obj);
                }

                poolDict.Add(pool.key, list);
                prefabDict.Add(pool.key, pool.prefab);
            }
        }

       

        // =========================
        // SPAWN (4 PARAM)
        // =========================
        public GameObject Spawn(string key, Vector3 pos, FruitType type, int level, bool playMergeScale = false, bool enableCollisionDelay = true)
        {
            Debug.Log("Spawn key: " + key);

            if (poolDict == null)
            {
                Debug.LogError("poolDict NULL");
            }

            if (!poolDict.ContainsKey(key))
            {
                Debug.LogError("Key KHÔNG tồn tại: " + key);
            }
            if (!poolDict.ContainsKey(key))
            {
                Debug.LogError("Pool key not found: " + key);
                return null;
            }

            var list = poolDict[key];

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].activeInHierarchy)
                {
                    return Setup(list[i], pos, type, level, playMergeScale, enableCollisionDelay);
                }
            }

            // grow đúng prefab
            GameObject obj = Instantiate(prefabDict[key]);
            list.Add(obj);

            return Setup(obj, pos, type, level, playMergeScale, enableCollisionDelay);
        }

        public Sprite GetSprite(FruitType type)
        {
            string key = type.ToString();
            if (prefabDict == null || !prefabDict.TryGetValue(key, out GameObject prefab) || prefab == null)
                return null;

            SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
            return spriteRenderer != null ? spriteRenderer.sprite : null;
        }

        GameObject Setup(GameObject obj, Vector3 pos, FruitType type, int level, bool playMergeScale, bool enableCollisionDelay)
        {
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = playMergeScale ? Vector3.zero : Vector3.one;

            Fruit fruit = obj.GetComponent<Fruit>();
            fruit.OnSpawn(type, level, pos, playMergeScale, enableCollisionDelay);

            return obj;
        }

        // =========================
        // DESPAWN
        // =========================
        public void Despawn(GameObject obj)
        {
            if (obj.TryGetComponent(out Fruit fruit))
                fruit.OnDespawn();
            else
                obj.SetActive(false);
        }
    }
}
