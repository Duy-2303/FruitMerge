using Link;
using Sirenix.OdinInspector;
using UnityEngine;
namespace DuyDZ.MergeFood.Test
{
    public class MergeManager : MonoBehaviour
    {
        public static MergeManager Ins;

        [Header("Merge FX")]
        [SerializeField] private ParticleSystem mergeParticlePrefab;
        [SerializeField] private Color mergeParticleStartColor = new Color(1f, 0.95f, 0.25f, 1f);
        [SerializeField] private Color mergeParticleEndColor = new Color(1f, 0.45f, 0.18f, 1f);
        [SerializeField] private int mergeParticleCount = 24;
        [SerializeField] private float mergeParticleLifeTime = 0.45f;
        [SerializeField] private Vector2 mergeParticleSpeed = new Vector2(1.6f, 3.1f);
        [SerializeField] private Vector2 mergeParticleSize = new Vector2(0.08f, 0.18f);
        [SerializeField] private float mergeParticleRadius = 0.22f;
        [SerializeField] private int mergeParticleSortingOrder = 80;

        private void Awake()
        {
            Ins = this;
            ScoreManager.GetOrCreate();

            if (GetComponent<FruitBoosterManager>() == null)
                gameObject.AddComponent<FruitBoosterManager>();
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
            ParticleSystem fruitMergeVfxPrefab = a.MergeVfxPrefab != null ? a.MergeVfxPrefab : b.MergeVfxPrefab;

            // ❗ lưu lại trước khi despawn (an toàn)
            GameObject objA = a.gameObject;
            GameObject objB = b.gameObject;

            ObjectPooler.current.Despawn(objA);
            ObjectPooler.current.Despawn(objB);

            if (nextLevel > (int)FruitType.Watermelon)
                return;

            string key = ((FruitType)nextLevel).ToString();

            ObjectPooler.current.Spawn(key, pos, (FruitType)nextLevel, nextLevel, true);
            PlayMergeParticle(pos, fruitMergeVfxPrefab);
            PlayMergeHaptic();
            ScoreManager.GetOrCreate().AddScore(nextLevel * 10);
        }

        private void PlayMergeParticle(Vector3 position, ParticleSystem fruitMergeVfxPrefab)
        {
            ParticleSystem prefab = fruitMergeVfxPrefab != null ? fruitMergeVfxPrefab : mergeParticlePrefab;
            if (prefab != null)
            {
                ParticleSystem particle = Instantiate(prefab, position, Quaternion.identity);
                particle.Play();
                Destroy(particle.gameObject, Fruit.GetParticleDestroyDelay(particle));
                return;
            }

            GameObject particleObject = new GameObject("MergeParticle");
            particleObject.transform.position = position;

            ParticleSystem particleSystem = particleObject.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particleSystem.main;
            main.startLifetime = mergeParticleLifeTime;
            main.startSpeed = new ParticleSystem.MinMaxCurve(mergeParticleSpeed.x, mergeParticleSpeed.y);
            main.startSize = new ParticleSystem.MinMaxCurve(mergeParticleSize.x, mergeParticleSize.y);
            main.startColor = new ParticleSystem.MinMaxGradient(mergeParticleStartColor, mergeParticleEndColor);
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.loop = false;

            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)mergeParticleCount) });

            ParticleSystem.ShapeModule shape = particleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = mergeParticleRadius;

            ParticleSystemRenderer particleRenderer = particleObject.GetComponent<ParticleSystemRenderer>();
            particleRenderer.sortingOrder = mergeParticleSortingOrder;

            ParticleSystem.VelocityOverLifetimeModule velocity = particleSystem.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;
            velocity.radial = 1.2f;

            ParticleSystem.ColorOverLifetimeModule color = particleSystem.colorOverLifetime;
            color.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(mergeParticleStartColor, 0f), new GradientColorKey(mergeParticleEndColor, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
            color.color = gradient;

            particleSystem.Play();
            Destroy(particleObject, mergeParticleLifeTime + 0.4f);
        }
        static void PlayMergeHaptic()
        {
            bool hapticEnabled = PlayerPrefs.GetInt("HapticEnabled", 1) == 1;
            if(hapticEnabled)
            {
                Handheld.Vibrate();
            }
        }
    }
}
