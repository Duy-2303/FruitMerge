using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DuyDZ.MergeFood.Test
{
    public class FruitBoosterManager : MonoBehaviour
    {
        private enum SelectionMode
        {
            None,
            Boom,
            LevelUp
        }

        private const string RemoveFruitsButtonName = "Button_RemoveFruits";
        private const string BoomFruitButtonName = "Button_BoomAFruit";
        private const string LevelUpFruitButtonName = "Button_LevelUpAFruit";

        private Button removeFruitsButton;
        private Button boomFruitButton;
        private Button levelUpFruitButton;
        private FruitSpawner fruitSpawner;
        private SelectionMode selectionMode;

        private void Start()
        {
            removeFruitsButton = FindButton(RemoveFruitsButtonName);
            boomFruitButton = FindButton(BoomFruitButtonName);
            levelUpFruitButton = FindButton(LevelUpFruitButtonName);
            fruitSpawner = FindFirstObjectByType<FruitSpawner>();

            Bind(removeFruitsButton, RemoveTwoSmallestFruitTypes);
            Bind(boomFruitButton, ToggleBoomSelection);
            Bind(levelUpFruitButton, ToggleLevelUpSelection);
        }

        private void OnDestroy()
        {
            Unbind(removeFruitsButton, RemoveTwoSmallestFruitTypes);
            Unbind(boomFruitButton, ToggleBoomSelection);
            Unbind(levelUpFruitButton, ToggleLevelUpSelection);
            SetSelectionMode(SelectionMode.None);
        }

        private void Update()
        {
            if (fruitSpawner != null && fruitSpawner.IsGameOver)
            {
                SetSelectionMode(SelectionMode.None);
                return;
            }

            if (selectionMode == SelectionMode.None || !Input.GetMouseButtonDown(0))
                return;

            if (IsPointerOverUI() || Camera.main == null)
                return;

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);
            if (hitCollider == null || !hitCollider.TryGetComponent(out Fruit fruit))
                return;

            if (!IsPlayableFruit(fruit))
                return;

            if (selectionMode == SelectionMode.Boom)
                BoomFruit(fruit);
            else
                LevelUpFruit(fruit);
        }

        private void RemoveTwoSmallestFruitTypes()
        {
            Fruit[] fruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
            int scoreToAdd = 0;
            bool removedAnyFruit = false;

            foreach (Fruit fruit in fruits)
            {
                if (!IsPlayableFruit(fruit) || fruit.type > FruitType.Blueberry)
                    continue;

                scoreToAdd += GetFruitScore(fruit);
                PlayFruitEffect(fruit);
                ObjectPooler.current.Despawn(fruit.gameObject);
                removedAnyFruit = true;
            }

            if (scoreToAdd > 0)
                ScoreManager.GetOrCreate().AddScore(scoreToAdd);

            if (removedAnyFruit)
                PlayHaptic();
        }

        private void ToggleBoomSelection()
        {
            SetSelectionMode(
                selectionMode == SelectionMode.Boom ? SelectionMode.None : SelectionMode.Boom);
        }

        private void ToggleLevelUpSelection()
        {
            SetSelectionMode(
                selectionMode == SelectionMode.LevelUp ? SelectionMode.None : SelectionMode.LevelUp);
        }

        private void BoomFruit(Fruit fruit)
        {
            int scoreToAdd = GetFruitScore(fruit);

            PlayFruitEffect(fruit);
            ObjectPooler.current.Despawn(fruit.gameObject);
            ScoreManager.GetOrCreate().AddScore(scoreToAdd);
            PlayHaptic();
            SetSelectionMode(SelectionMode.None);
        }

        private void LevelUpFruit(Fruit fruit)
        {
            int nextLevel = fruit.level + 1;
            if (nextLevel > (int)FruitType.Watermelon)
                return;

            int scoreToAdd = GetFruitScore(fruit);
            Vector3 position = fruit.TF.position;
            ParticleSystem effectPrefab = fruit.MergeVfxPrefab;

            ObjectPooler.current.Despawn(fruit.gameObject);
            FruitType nextType = (FruitType)nextLevel;
            ObjectPooler.current.Spawn(
                nextType.ToString(),
                position,
                nextType,
                nextLevel,
                true);

            PlayFruitEffect(position, effectPrefab);
            ScoreManager.GetOrCreate().AddScore(scoreToAdd);
            PlayHaptic();
            SetSelectionMode(SelectionMode.None);
        }

        private void SetSelectionMode(SelectionMode mode)
        {
            selectionMode = mode;

            if (fruitSpawner != null)
                fruitSpawner.IsInputLocked = mode != SelectionMode.None;

            SetButtonSelected(boomFruitButton, mode == SelectionMode.Boom);
            SetButtonSelected(levelUpFruitButton, mode == SelectionMode.LevelUp);
        }

        private static int GetFruitScore(Fruit fruit)
        {
            return (fruit.level + 1) * 10;
        }

        private static bool IsPlayableFruit(Fruit fruit)
        {
            if (fruit == null || !fruit.gameObject.activeInHierarchy || fruit.IsMerging)
                return false;

            Rigidbody2D rigidbody2D = fruit.GetComponent<Rigidbody2D>();
            return rigidbody2D != null && rigidbody2D.bodyType == RigidbodyType2D.Dynamic;
        }

        private static void PlayFruitEffect(Fruit fruit)
        {
            PlayFruitEffect(fruit.TF.position, fruit.MergeVfxPrefab);
        }

        private static void PlayFruitEffect(Vector3 position, ParticleSystem effectPrefab)
        {
            if (effectPrefab == null)
                return;

            ParticleSystem effect = Instantiate(effectPrefab, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, Fruit.GetParticleDestroyDelay(effect));
        }

        private static void PlayHaptic()
        {
            if (PlayerPrefs.GetInt("HapticEnabled", 1) == 1)
                Handheld.Vibrate();
        }

        private static bool IsPointerOverUI()
        {
            if (EventSystem.current == null)
                return false;

            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

            return EventSystem.current.IsPointerOverGameObject();
        }

        private static Button FindButton(string objectName)
        {
            GameObject buttonObject = GameObject.Find(objectName);
            return buttonObject != null ? buttonObject.GetComponent<Button>() : null;
        }

        private static void Bind(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button != null)
                button.onClick.AddListener(action);
        }

        private static void Unbind(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button != null)
                button.onClick.RemoveListener(action);
        }

        private static void SetButtonSelected(Button button, bool selected)
        {
            if (button == null || button.targetGraphic == null)
                return;

            button.targetGraphic.color = selected
                ? new Color(1f, 0.85f, 0.25f, 1f)
                : Color.white;
        }
    }
}
