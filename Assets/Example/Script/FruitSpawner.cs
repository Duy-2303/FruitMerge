using Link;
using System;
using System.Collections.Generic;
using DuyDZ.MergeFood;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DuyDZ.MergeFood.Test
{
    public class FruitSpawner : MonoBehaviour
    {
        public Transform spawnPoint;
        [SerializeField] private GameObject settingsPanel;
        [Header("Game Over")]
        [SerializeField] private float gameOverContactDuration = 1f;
        [SerializeField] private float gameOverPopupDelay = 2f;
        [SerializeField] private EndGamePopUpUi gameOverPopup;
        [SerializeField] private float validClickBelowSpawnOffset = 0.05f;
        [Header("Screen Bounds")]
        [SerializeField] private bool clampFruitToScreen = true;
        [SerializeField] private float screenEdgePadding = 0.02f;
        public FruitType NextFruitType => nextFruitType;
        public event Action<FruitType> OnNextFruitChanged;

        private GameObject currentFruit;
        private bool isDragging;
        private bool canSpawn = true;
        private FruitType nextFruitType;
        private bool hasNextFruit;
        private Vector3 lastSpawnPosition;
        public bool IsInputLocked { get; set; }
        public bool IsGameOver { get; private set; }

        private void Awake()
        {
            Time.timeScale = 1f;
            lastSpawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            SetupSpawnLineGameOverDetector();
            PrepareNextFruit();
        }

        private void Start()
        {
            SpawnNew();
        }

        private void Update()
        {
            if (IsGameOver ||
                IsInputLocked ||
                (settingsPanel != null && settingsPanel.activeInHierarchy) ||
                IsPointerOverUI())
            {
                isDragging = false;
                return;
            }

            if (Input.GetMouseButtonDown(0) && canSpawn && IsPointerBelowSpawnBar())
            {
                if (currentFruit == null)
                    SpawnNew();

                isDragging = true;
            }

            if (Input.GetMouseButton(0) && isDragging && currentFruit != null)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                pos.y = spawnPoint.position.y;
                pos.z = 0;
                pos = ClampToScreen(pos, currentFruit);

                currentFruit.transform.position = pos;
            }

            if (Input.GetMouseButtonUp(0) && isDragging && currentFruit != null)
            {
                isDragging = false;
                Drop();
            }
        }

        private void SetupSpawnLineGameOverDetector()
        {
            if (spawnPoint == null)
                return;

            SpawnLineGameOver detector = spawnPoint.GetComponent<SpawnLineGameOver>();
            if (detector == null)
                detector = spawnPoint.gameObject.AddComponent<SpawnLineGameOver>();

            detector.Initialize(this, gameOverContactDuration);
        }

        public void TriggerGameOver()
        {
            if (IsGameOver)
                return;

            IsGameOver = true;
            IsInputLocked = true;
            isDragging = false;
            canSpawn = false;
            CancelInvoke(nameof(ResetSpawn));

            Debug.Log("Game Over: Fruit touched the spawn line.");

            if (gameOverPopupDelay <= 0f)
                ShowGameOverPopup();
            else
                Invoke(nameof(ShowGameOverPopup), gameOverPopupDelay);
        }

        public void ShowGameOverPopup()
        {
            if (gameOverPopup != null)
            {
                gameOverPopup.Show();
                return;
            }

            Debug.LogWarning("PopUpEnd has not been assigned to FruitSpawner.", this);
        }

        private static bool IsPointerOverUI()
        {
            if (EventSystem.current == null)
                return false;

            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

            return EventSystem.current.IsPointerOverGameObject();
        }

        private void Drop()
        {
            Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
            Fruit fruit = currentFruit.GetComponent<Fruit>();
            lastSpawnPosition = currentFruit.transform.position;
            lastSpawnPosition.y = spawnPoint.position.y;
            lastSpawnPosition.z = 0f;

            if (fruit != null)
                fruit.SetColliderEnabled(true);

            rb.bodyType = RigidbodyType2D.Dynamic;

            currentFruit = null;
            canSpawn = false;

            Invoke(nameof(ResetSpawn), 0.2f);
        }

        private void ResetSpawn()
        {
            if (IsGameOver)
                return;

            canSpawn = true;

            if (currentFruit == null)
                SpawnNew();
        }

        private void SpawnNew()
        {
            if (IsGameOver)
                return;

            if (!hasNextFruit)
                PrepareNextFruit();

            FruitType type = nextFruitType;
            PrepareNextFruit();

            string key = type.ToString();

            currentFruit = ObjectPooler.current.Spawn(
                key,
                GetSpawnPosition(),
                type,
                (int)type,
                false,
                false);

            Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private FruitType GetRandomType()
        {
            return (FruitType)UnityEngine.Random.Range(0, 3);
        }

        private void PrepareNextFruit()
        {
            nextFruitType = GetRandomType();
            hasNextFruit = true;
            OnNextFruitChanged?.Invoke(nextFruitType);
        }

        private Vector3 GetSpawnPosition()
        {
            Vector3 spawnPosition = lastSpawnPosition;
            if (spawnPoint != null)
                spawnPosition.y = spawnPoint.position.y;

            spawnPosition.z = 0f;
            return ClampToScreen(spawnPosition, null);
        }

        private bool IsPointerBelowSpawnBar()
        {
            if (spawnPoint == null || Camera.main == null)
                return true;

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return worldPosition.y <= spawnPoint.position.y - validClickBelowSpawnOffset;
        }

        private Vector3 ClampToScreen(Vector3 worldPosition, GameObject fruit)
        {
            if (!clampFruitToScreen || Camera.main == null)
                return worldPosition;

            Camera camera = Camera.main;
            float cameraDistance = Mathf.Abs(camera.transform.position.z - worldPosition.z);
            Vector3 min = camera.ScreenToWorldPoint(new Vector3(0f, 0f, cameraDistance));
            Vector3 max = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cameraDistance));

            Vector2 extent = GetFruitExtent(fruit);
            float minX = Mathf.Min(min.x, max.x) + extent.x + screenEdgePadding;
            float maxX = Mathf.Max(min.x, max.x) - extent.x - screenEdgePadding;
            float minY = Mathf.Min(min.y, max.y) + extent.y + screenEdgePadding;
            float maxY = Mathf.Max(min.y, max.y) - extent.y - screenEdgePadding;

            worldPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);
            worldPosition.y = Mathf.Clamp(worldPosition.y, minY, maxY);
            return worldPosition;
        }

        private Vector2 GetFruitExtent(GameObject fruit)
        {
            if (fruit == null)
                return Vector2.zero;

            Collider2D collider2D = fruit.GetComponent<Collider2D>();
            if (collider2D == null)
                return Vector2.zero;

            return collider2D.bounds.extents;
        }
    }

    public class SpawnLineGameOver : MonoBehaviour
    {
        private readonly Dictionary<Fruit, float> fruitContactStartTimes =
            new Dictionary<Fruit, float>();

        private FruitSpawner fruitSpawner;
        private float requiredContactDuration;

        public void Initialize(FruitSpawner owner, float contactDuration)
        {
            fruitSpawner = owner;
            requiredContactDuration = Mathf.Max(0f, contactDuration);

            BoxCollider2D trigger = GetComponent<BoxCollider2D>();
            if (trigger == null)
                trigger = gameObject.AddComponent<BoxCollider2D>();

            trigger.isTrigger = true;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                Bounds spriteBounds = spriteRenderer.sprite.bounds;
                trigger.size = spriteBounds.size;
                trigger.offset = spriteBounds.center;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Fruit fruit))
                fruitContactStartTimes[fruit] = Time.time;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (fruitSpawner == null || fruitSpawner.IsGameOver)
                return;

            if (!other.TryGetComponent(out Fruit fruit) ||
                fruit.IsMerging ||
                !fruit.gameObject.activeInHierarchy)
            {
                return;
            }

            Rigidbody2D rigidbody2D = fruit.GetComponent<Rigidbody2D>();
            if (rigidbody2D == null || rigidbody2D.bodyType != RigidbodyType2D.Dynamic)
                return;

            if (!fruitContactStartTimes.TryGetValue(fruit, out float contactStartTime))
            {
                fruitContactStartTimes[fruit] = Time.time;
                return;
            }

            if (Time.time - contactStartTime >= requiredContactDuration)
                fruitSpawner.TriggerGameOver();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Fruit fruit))
                fruitContactStartTimes.Remove(fruit);
        }

        private void OnDisable()
        {
            fruitContactStartTimes.Clear();
        }
       
    }
}
