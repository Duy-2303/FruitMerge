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
        [Header("Spawn Animation")]
        [SerializeField] private float scalePopSize = 1.12f;
        [SerializeField] private float scalePopDuration = 0.14f;
        [SerializeField] private float mergeScaleStart = 0.2f;
        [SerializeField] private float mergeScaleDuration = 0.2f;
        [Header("Physics")]
        [SerializeField] private float fallGravityScale = 2.6f;
        [Header("VFX")]
        [SerializeField] private ParticleSystem mergeVfxPrefab;

        private bool isMerging;
        private Coroutine scaleRoutine;
        private Coroutine collisionRoutine;
        public bool IsMerging => isMerging;
        public ParticleSystem MergeVfxPrefab => mergeVfxPrefab;

        public Transform TF => transform;

       

        // =========================
        // SPAWN / DESPAWN
        // =========================

        public void OnSpawn(FruitType type, int level, Vector3 pos, bool playMergeScale = false, bool enableCollisionDelay = true)
        {
            this.type = type;
            this.level = level;
            rb2D.mass = 1 + level * 0.5f;
            rb2D.gravityScale = fallGravityScale;
            TF.position = pos;
            TF.rotation = Quaternion.identity;
            TF.localScale = playMergeScale ? Vector3.one * mergeScaleStart : Vector3.one;

            isMerging = false;

            // reset physics
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0;
            rb2D.bodyType = RigidbodyType2D.Dynamic;

            gameObject.SetActive(true);

            if (collisionRoutine != null)
                StopCoroutine(collisionRoutine);

            if (enableCollisionDelay)
                collisionRoutine = StartCoroutine(EnableCollisionDelay());
            else
                SetColliderEnabled(false);

            if (playMergeScale)
                PlayMergeScale();
            else
                PlayScaleLight();
        }

        public void OnDespawn()
        {
            if (scaleRoutine != null)
            {
                StopCoroutine(scaleRoutine);
                scaleRoutine = null;
            }

            if (collisionRoutine != null)
            {
                StopCoroutine(collisionRoutine);
                collisionRoutine = null;
            }

            gameObject.SetActive(false);
        }

        private IEnumerator EnableCollisionDelay()
        {
            SetColliderEnabled(false);
            yield return new WaitForSeconds(0.05f);
            SetColliderEnabled(true);
            collisionRoutine = null;
        }

        public void SetColliderEnabled(bool enabled)
        {
            if (col2D != null)
                col2D.enabled = enabled;
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

        private void PlayScaleLight()
        {
            if (scaleRoutine != null)
                StopCoroutine(scaleRoutine);

            scaleRoutine = StartCoroutine(ScaleLightRoutine());
        }

        private void PlayMergeScale()
        {
            if (scaleRoutine != null)
                StopCoroutine(scaleRoutine);

            scaleRoutine = StartCoroutine(MergeScaleRoutine());
        }

        public static float GetParticleDestroyDelay(ParticleSystem particle)
        {
            ParticleSystem.MainModule main = particle.main;
            return main.duration + main.startLifetime.constantMax + 0.2f;
        }

        private IEnumerator ScaleLightRoutine()
        {
            Vector3 normalScale = Vector3.one;
            Vector3 popScale = Vector3.one * scalePopSize;
            float halfDuration = scalePopDuration * 0.5f;

            yield return ScaleTo(normalScale, popScale, halfDuration);
            yield return ScaleTo(popScale, normalScale, halfDuration);

            TF.localScale = normalScale;
            scaleRoutine = null;
        }

        private IEnumerator MergeScaleRoutine()
        {
            Vector3 startScale = Vector3.one * mergeScaleStart;
            Vector3 targetScale = Vector3.one;

            yield return ScaleTo(startScale, targetScale, mergeScaleDuration);

            TF.localScale = targetScale;
            scaleRoutine = null;
        }

        private IEnumerator ScaleTo(Vector3 from, Vector3 to, float duration)
        {
            if (duration <= 0f)
            {
                TF.localScale = to;
                yield break;
            }

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                float easedT = 1f - Mathf.Pow(1f - t, 2f);
                TF.localScale = Vector3.LerpUnclamped(from, to, easedT);
                yield return null;
            }

            TF.localScale = to;
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
