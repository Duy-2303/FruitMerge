using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace DuyDZ.MergeFood
{
    public class ObjectPooler : MonoBehaviour
    {

        public static ObjectPooler current;

        [System.Serializable]
        public class Pool
        {
            public string key;
            public GameObject prefab;
            public int amount;
        }

        public List<Pool> pools;

        private Dictionary<string, List<GameObject>> poolDict;

        private void Awake()
        {
            current = this;
        }

        void Start()
        {
            poolDict = new Dictionary<string, List<GameObject>>();

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
            }
        }

        public GameObject GetPoolObject(string key, Vector3 pos)
        {
            if (!poolDict.ContainsKey(key)) return null;

            var list = poolDict[key];

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].activeInHierarchy)
                {
                    SetupObject(list[i], pos);
                    return list[i];
                }
            }

            // grow
            GameObject obj = Instantiate(list[0]);
            list.Add(obj);

            SetupObject(obj, pos);
            return obj;
        }

        void SetupObject(GameObject obj, Vector3 pos)
        {
            obj.SetActive(true);
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.identity;

            // reset physics
            if (obj.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            }
        }

        public void ReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
        }
    }


}
