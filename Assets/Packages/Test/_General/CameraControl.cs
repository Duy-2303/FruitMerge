using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Link
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraControl : Singleton<CameraControl>
    {
        public enum ShakeType
        {
            Light_1,
            Light_2,
            Normal_1,
            Normal_2,
            Heavy_1,
            Heavy_2
        }

        [Header("Auto Fit")]
        [FormerlySerializedAs("camera")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private bool autoFit = true;
        [SerializeField] private Vector2 referenceResolution = new Vector2(1080f, 1920f);
        [FormerlySerializedAs("min")]
        [SerializeField, Min(0.01f)] private float referenceOrthographicSize = 5f;

        [Header("Shake")]
        [SerializeField] private ShakeType testShakeType = ShakeType.Normal_1;
        [SerializeField, Min(0f)] private float strength = 0.2f;
        [SerializeField, Min(1)] private int vibrato = 10;
        [SerializeField, Range(0f, 180f)] private float randomness = 90f;

        private Transform cachedTransform;
        private Tween shakeTween;
        private Tween sizeTween;
        private Tween moveTween;
        private int lastPixelWidth = -1;
        private int lastPixelHeight = -1;

        public Transform TF => cachedTransform != null
            ? cachedTransform
            : cachedTransform = transform;

        public float orthographicSize => targetCamera != null
            ? targetCamera.orthographicSize
            : 0f;

        private void Awake()
        {
            _instance = this;
            CacheCamera();
        }

        private void OnEnable()
        {
            AutoFit();
        }

        private void Start()
        {
            // Chạy lại sau frame khởi tạo để Game View đã có đúng kích thước.
            AutoFit();
        }

        private void Update()
        {
            if (!autoFit || targetCamera == null)
                return;

            int width = GetPixelWidth();
            int height = GetPixelHeight();

            if (width != lastPixelWidth || height != lastPixelHeight)
                AutoFit();
        }

        [ContextMenu("Auto Fit Camera")]
        public void AutoFit()
        {
            CacheCamera();

            if (!autoFit || targetCamera == null || !targetCamera.orthographic)
                return;

            int width = GetPixelWidth();
            int height = GetPixelHeight();

            if (width <= 0 || height <= 0 ||
                referenceResolution.x <= 0f || referenceResolution.y <= 0f)
                return;

            float currentAspect = (float)width / height;
            float referenceAspect = referenceResolution.x / referenceResolution.y;

            // Giữ đủ chiều cao chuẩn và tăng size khi màn hình hẹp hơn thiết kế.
            float sizeToFitWidth = referenceOrthographicSize
                                   * referenceAspect
                                   / currentAspect;

            targetCamera.orthographicSize = Mathf.Max(
                referenceOrthographicSize,
                sizeToFitWidth);

            lastPixelWidth = width;
            lastPixelHeight = height;
        }

        public void OnShake(
            ShakeType shakeType,
            float duration = 0.5f,
            float delay = 0f)
        {
            ApplyShakePreset(shakeType);

            // Complete giúp camera trở về đúng vị trí trước khi rung lần tiếp theo.
            shakeTween?.Complete();
            shakeTween = TF
                .DOShakePosition(
                    duration,
                    strength,
                    vibrato,
                    randomness,
                    false,
                    true)
                .SetDelay(delay);
        }

        public void OnSize(
            float target,
            float duration = 0.5f,
            float delay = 0f)
        {
            CacheCamera();

            if (targetCamera == null)
                return;

            sizeTween?.Kill();
            sizeTween = targetCamera
                .DOOrthoSize(target, duration)
                .SetDelay(delay);
        }

        public void OnMove(
            Vector3 target,
            float duration = 0.5f,
            float delay = 0f)
        {
            moveTween?.Kill();
            moveTween = TF
                .DOMove(target, duration)
                .SetDelay(delay);
        }

        [ContextMenu("Test Shake")]
        private void TestShake()
        {
            OnShake(testShakeType);
        }

        private void CacheCamera()
        {
            if (targetCamera == null)
                targetCamera = GetComponent<Camera>();
        }

        private int GetPixelWidth()
        {
            return targetCamera != null && targetCamera.pixelWidth > 0
                ? targetCamera.pixelWidth
                : Screen.width;
        }

        private int GetPixelHeight()
        {
            return targetCamera != null && targetCamera.pixelHeight > 0
                ? targetCamera.pixelHeight
                : Screen.height;
        }

        private void ApplyShakePreset(ShakeType shakeType)
        {
            switch (shakeType)
            {
                case ShakeType.Light_1:
                    strength = 0.2f;
                    vibrato = 10;
                    break;
                case ShakeType.Light_2:
                    strength = 0.5f;
                    vibrato = 15;
                    break;
                case ShakeType.Normal_1:
                    strength = 1f;
                    vibrato = 20;
                    break;
                case ShakeType.Normal_2:
                    strength = 1.5f;
                    vibrato = 25;
                    break;
                case ShakeType.Heavy_1:
                    strength = 2f;
                    vibrato = 30;
                    break;
                case ShakeType.Heavy_2:
                    strength = 3f;
                    vibrato = 35;
                    break;
            }

            randomness = 90f;
        }

        private void OnDestroy()
        {
            shakeTween?.Kill();
            sizeTween?.Kill();
            moveTween?.Kill();

            if (_instance == this)
                _instance = null;
        }
    }
}
