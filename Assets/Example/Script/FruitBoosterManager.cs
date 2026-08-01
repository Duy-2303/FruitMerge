using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
namespace DuyDZ.MergeFood.Test
{
    public class FruitBoosterManager : MonoBehaviour
    {
        public enum BoosterType
        {
            RemoveFruits,
            BoomFruit,
            LevelUpFruit
        }

        private sealed class BoosterState
        {
            public Button Button;
            public GameObject CountIcon;
            public GameObject AdsIcon;
            public TMP_Text CountText;
            public string PlayerPrefsKey;
            public int RemainingUses;
        }

        private enum SelectionMode
        {
            None,
            Boom,
            LevelUp
        }

        private const string RemoveFruitsButtonName = "Button_RemoveFruits";
        private const string BoomFruitButtonName = "Button_BoomAFruit";
        private const string LevelUpFruitButtonName = "Button_LevelUpAFruit";
        private const int InitialBoosterUses = 3;

        private Button removeFruitsButton;
        private Button boomFruitButton;
        private Button levelUpFruitButton;
        private FruitSpawner fruitSpawner;
        private SelectionMode selectionMode;
        private BoosterState removeFruitsBooster;
        private BoosterState boomFruitBooster;
        private BoosterState levelUpFruitBooster;

        private void Start()
        {
            removeFruitsButton = FindButton(RemoveFruitsButtonName);
            boomFruitButton = FindButton(BoomFruitButtonName);
            levelUpFruitButton = FindButton(LevelUpFruitButtonName);
            fruitSpawner = FindFirstObjectByType<FruitSpawner>();

            removeFruitsBooster = CreateBoosterState(removeFruitsButton, BoosterType.RemoveFruits);
            boomFruitBooster = CreateBoosterState(boomFruitButton, BoosterType.BoomFruit);
            levelUpFruitBooster = CreateBoosterState(levelUpFruitButton, BoosterType.LevelUpFruit);

            Bind(removeFruitsButton, OnRemoveFruitsPressed);
            Bind(boomFruitButton, OnBoomFruitPressed);
            Bind(levelUpFruitButton, OnLevelUpFruitPressed);
        }

        private void OnDestroy()
        {
            Unbind(removeFruitsButton, OnRemoveFruitsPressed);
            Unbind(boomFruitButton, OnBoomFruitPressed);
            Unbind(levelUpFruitButton, OnLevelUpFruitPressed);
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

        private void OnRemoveFruitsPressed()
        {
            if (!TryStartBooster(BoosterType.RemoveFruits, removeFruitsBooster))
                return;

            if (RemoveTwoSmallestFruitTypes())
                ConsumeUse(removeFruitsBooster);
        }

        private void OnBoomFruitPressed()
        {
            if (TryStartBooster(BoosterType.BoomFruit, boomFruitBooster))
                ToggleBoomSelection();
        }

        private void OnLevelUpFruitPressed()
        {
            if (TryStartBooster(BoosterType.LevelUpFruit, levelUpFruitBooster))
                ToggleLevelUpSelection();
        }

        private bool RemoveTwoSmallestFruitTypes()
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

            return removedAnyFruit;
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
            ConsumeUse(boomFruitBooster);
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
            ConsumeUse(levelUpFruitBooster);
            SetSelectionMode(SelectionMode.None);
        }

        private bool TryStartBooster(BoosterType boosterType, BoosterState state)
        {
            if (state != null && state.RemainingUses > 0)
                return true;

            SetSelectionMode(SelectionMode.None);
            ShowRewardedAd(boosterType, () => GrantBoosterUse(state));
            return false;
        }

        // Chèn SDK quảng cáo rewarded của bạn tại đây và chỉ gọi onRewardGranted
        // sau khi người chơi đã xem quảng cáo thành công.
        protected virtual void ShowRewardedAd(BoosterType boosterType, Action onRewardGranted)
        {
            GoogleAdsManager ads =        GoogleAdsManager.Instance;
            if (ads == null)
            {
                return;
            }
            if (!ads.IsRewardedReady())
            {
                Debug.LogWarning(
                    $"Rewarded chưa sẵn sàng cho {boosterType}.");

                ads.LoadRewarded();
                return;
            }

            ads.ShowRewarded(() =>
            {
                Debug.Log(
                    $"Người chơi đã nhận thưởng cho {boosterType}.");

                onRewardGranted?.Invoke();
            });

        }

            private static void GrantBoosterUse(BoosterState state)
        {
            if (state == null)
                return;

            state.RemainingUses++;
            SaveBoosterUses(state);
            RefreshBoosterUI(state);
        }

        private static void ConsumeUse(BoosterState state)
        {
            if (state == null || state.RemainingUses <= 0)
                return;

            state.RemainingUses--;
            SaveBoosterUses(state);
            RefreshBoosterUI(state);
        }

        private static BoosterState CreateBoosterState(Button button, BoosterType boosterType)
        {
            if (button == null)
                return null;

            Transform countTransform = button.transform.Find("Count");
            Transform adsTransform = button.transform.Find("Ads");
            string playerPrefsKey = $"BoosterUses_{boosterType}";
            BoosterState state = new BoosterState
            {
                Button = button,
                CountIcon = countTransform != null ? countTransform.gameObject : null,
                AdsIcon = adsTransform != null ? adsTransform.gameObject : null,
                PlayerPrefsKey = playerPrefsKey,
                RemainingUses = Mathf.Max(0, PlayerPrefs.GetInt(playerPrefsKey, InitialBoosterUses))
            };

            if (countTransform != null)
                state.CountText = CreateCountText(countTransform);

            RefreshBoosterUI(state);
            return state;
        }

        private static void SaveBoosterUses(BoosterState state)
        {
            PlayerPrefs.SetInt(state.PlayerPrefsKey, state.RemainingUses);
            PlayerPrefs.Save();
        }

        private static TMP_Text CreateCountText(Transform countTransform)
        {
            GameObject textObject = new GameObject("Value", typeof(RectTransform), typeof(CanvasRenderer));
            textObject.transform.SetParent(countTransform, false);

            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 30f;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        private static void RefreshBoosterUI(BoosterState state)
        {
            bool hasUses = state.RemainingUses > 0;

            if (state.CountIcon != null)
                state.CountIcon.SetActive(hasUses);
            if (state.AdsIcon != null)
                state.AdsIcon.SetActive(!hasUses);
            if (state.CountText != null)
                state.CountText.text = state.RemainingUses.ToString();
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
