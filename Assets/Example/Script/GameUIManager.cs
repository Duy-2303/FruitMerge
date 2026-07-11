using DuyDZ.MergeFood;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DuyDZ.MergeFood.Test
{
    public class GameUIManager : MonoBehaviour
    {
        private const string AudioMutedKey = "FruitMergeAudioMuted";

        [Header("Scene References")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text bestScoreText;
        [SerializeField] private TMP_Text soundText;
        [SerializeField] private Image nextFruitImage;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button exitButton;

        private FruitSpawner fruitSpawner;
        private bool isMuted;

        private void Awake()
        {
            isMuted = PlayerPrefs.GetInt(AudioMutedKey, 0) == 1;
            ApplyAudioState();
            BindButtons();
            RefreshSoundText();
        }

        private void Start()
        {
            ScoreManager scoreManager = ScoreManager.GetOrCreate();
            scoreManager.BindScoreTexts(scoreText, bestScoreText);
            scoreManager.OnScoreChanged += UpdateScoreTexts;
            UpdateScoreTexts(scoreManager.CurrentScore, scoreManager.BestScore);

            fruitSpawner = FindObjectOfType<FruitSpawner>();
            if (fruitSpawner != null)
            {
                fruitSpawner.OnNextFruitChanged += UpdateNextFruit;
                UpdateNextFruit(fruitSpawner.NextFruitType);
            }
        }

        private void OnDestroy()
        {
            ScoreManager scoreManager = ScoreManager.Ins;
            if (scoreManager != null)
                scoreManager.OnScoreChanged -= UpdateScoreTexts;

            if (fruitSpawner != null)
                fruitSpawner.OnNextFruitChanged -= UpdateNextFruit;
        }

        private void BindButtons()
        {
            if (settingsButton != null)
                settingsButton.onClick.AddListener(ToggleSettings);

            if (soundButton != null)
                soundButton.onClick.AddListener(ToggleAudio);

            if (exitButton != null)
                exitButton.onClick.AddListener(ExitGame);
        }

        private void UpdateScoreTexts(int currentScore, int bestScore)
        {
            if (scoreText != null)
                scoreText.text = currentScore.ToString();

            if (bestScoreText != null)
                bestScoreText.text = bestScore.ToString();
        }

        private void UpdateNextFruit(FruitType fruitType)
        {
            if (nextFruitImage == null || ObjectPooler.current == null)
                return;

            nextFruitImage.sprite = ObjectPooler.current.GetSprite(fruitType);
            nextFruitImage.enabled = nextFruitImage.sprite != null;
        }

        private void ToggleSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(!settingsPanel.activeSelf);
        }

        private void ToggleAudio()
        {
            isMuted = !isMuted;
            PlayerPrefs.SetInt(AudioMutedKey, isMuted ? 1 : 0);
            PlayerPrefs.Save();
            ApplyAudioState();
            RefreshSoundText();
        }

        private void ApplyAudioState()
        {
            AudioListener.volume = isMuted ? 0f : 1f;

            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                float volume = isMuted ? 0f : 1f;
                audioManager.SetMusicVolume(volume);
                audioManager.SetSFXVolume(volume);
            }
        }

        private void RefreshSoundText()
        {
            if (soundText != null)
                soundText.text = isMuted ? "SOUND: OFF" : "SOUND: ON";
        }

        private void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
