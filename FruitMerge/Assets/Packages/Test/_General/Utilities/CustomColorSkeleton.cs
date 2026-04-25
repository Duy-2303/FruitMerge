using System.Collections;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace Link.Cooking
{
    public class CustomColorSkeleton : MonoBehaviour
    {
        [SerializeField] private SkeletonRenderer skeleton;
        [SerializeField] private Color color;

        public void SetColor(Color c)
        {
            color = c;
            skeleton.skeleton.SetColor(color);
        }

        public void DoColor(Color c, float time, float delay = 0)
        {
            color = c;
            StartCoroutine(IEDoColor(c, time, delay));
        }

        [Button]
        public void SetColor()
        {
            skeleton.skeleton.SetColor(color);
        }

        private IEnumerator IEDoColor(Color c, float time, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            Color startColor = skeleton.skeleton.GetColor();
            float t = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                if (t >= time) t = time;
                skeleton.skeleton.SetColor(Color.Lerp(startColor, c, t / time));
                yield return null;
            }
        }
    }
}