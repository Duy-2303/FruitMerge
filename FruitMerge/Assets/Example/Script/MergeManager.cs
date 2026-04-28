using Link;
using Sirenix.OdinInspector;
using UnityEngine;
namespace DuyDZ.MergeFood.Test
{
    public class MergeManager : MonoBehaviour
    {
        public static MergeManager Ins;

        private void Awake()
        {
            Ins = this;
        }

        public void RequestMerge(Fruit a, Fruit b)
        {
            if (a == null || b == null) return;
            if (a == b) return;

            // tránh double merge
            if (a.IsMerging || b.IsMerging) return;

            a.SetMerging();
            b.SetMerging();

            Vector3 pos = (a.TF.position + b.TF.position) / 2f;
            int nextLevel = a.level + 1;

            // ❗ lưu lại trước khi despawn (an toàn)
            GameObject objA = a.gameObject;
            GameObject objB = b.gameObject;

            ObjectPooler.current.Despawn(objA);
            ObjectPooler.current.Despawn(objB);

            if (nextLevel > (int)FruitType.Watermelon)
                return;

            string key = ((FruitType)nextLevel).ToString();

            ObjectPooler.current.Spawn(key, pos, (FruitType)nextLevel, nextLevel);

            //ScoreManager.Ins.AddScore(nextLevel * 10);
        }
    }
}