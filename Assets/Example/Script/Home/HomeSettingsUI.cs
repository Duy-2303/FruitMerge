using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DuyDZ.MergeFood
{
    /// <summary>
    /// Connects the Home scene's settings artwork to the game's saved settings.
    /// References are resolved by name so the UI can still be rearranged in the editor.
    /// </summary>
    public sealed class HomeSettingsUI : MonoBehaviour
    {
        private const string MusicVolumeKey = "MusicVolume";
        private const string SfxVolumeKey = "SFXVolume";
        private const string HapticEnabledKey = "HapticEnabled";
        private const string HomeSceneName = "HomeScene";
        private const string GameplaySceneName = "BuildScene";

        private GameObject settingsPanel;
        private Button openButton;
        private Button closeButton;
        private Button musicButton;
        private Button soundButton;
        private Button hapticButton;
        private Button restartButton;
        private Button homeButton;
        private Button playButton;

        private void Awake()
        {
            ResolveReferences();
            BindButtons();
            ApplySavedAudioSettings();
            RefreshAllIndicators();

            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            Unbind(openButton, OpenSettings);
            Unbind(closeButton, CloseSettings);
            Unbind(musicButton, ToggleMusic);
            Unbind(soundButton, ToggleSound);
            Unbind(hapticButton, ToggleHaptic);
            Unbind(restartButton, RestartGame);
            Unbind(homeButton, GoHome);
            Unbind(playButton, StartGame);
        }

        private void ResolveReferences()
        {
            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                switch (button.name)
                {
                    case "Setting":
                        openButton = button;
                        break;
                    case "Close":
                        closeButton = button;
                        break;
                    case "Button-Music":
                        musicButton = button;
                        break;
                    case "Button-Sound":
                        soundButton = button;
                        break;
                    case "Button-Hapic":
                        hapticButton = button;
                        break;
                    case "Button-restart":
                        restartButton = button;
                        break;
                    case "Button-Home":
                        homeButton = button;
                        break;
                    case "Play_Button":
                        playButton = button;
                        break;
                }
            }

            if (closeButton != null)
                settingsPanel = closeButton.transform.parent.gameObject;
        }

        private void BindButtons()
        {
            Bind(openButton, OpenSettings);
            Bind(closeButton, CloseSettings);
            Bind(musicButton, ToggleMusic);
            Bind(soundButton, ToggleSound);
            Bind(hapticButton, ToggleHaptic);
            Bind(restartButton, RestartGame);
            Bind(homeButton, GoHome);
            Bind(playButton, StartGame);
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

        private void OpenSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        private void CloseSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private void ToggleMusic()
        {
            SetVolume(MusicVolumeKey, !IsEnabled(MusicVolumeKey));
            ApplySavedAudioSettings();
            RefreshIndicator(musicButton, IsEnabled(MusicVolumeKey));
        }

        private void ToggleSound()
        {
            SetVolume(SfxVolumeKey, !IsEnabled(SfxVolumeKey));
            ApplySavedAudioSettings();
            RefreshIndicator(soundButton, IsEnabled(SfxVolumeKey));
        }

        private void ToggleHaptic()
        {
            bool enabled = PlayerPrefs.GetInt(HapticEnabledKey, 1) == 1;
            PlayerPrefs.SetInt(HapticEnabledKey, enabled ? 0 : 1);
            PlayerPrefs.Save();
            RefreshIndicator(hapticButton, !enabled);

            if (!enabled)
                Handheld.Vibrate();
        }

        private static bool IsEnabled(string key)
        {
            return PlayerPrefs.GetFloat(key, 1f) > 0.1f;
        }

        private static void SetVolume(string key, bool enabled)
        {
            PlayerPrefs.SetFloat(key, enabled ? 1f : 0f);
            PlayerPrefs.Save();
        }

        private static void ApplySavedAudioSettings()
        {
            AudioManager manager = FindFirstObjectByType<AudioManager>();
            if (manager == null)
                return;

            manager.SetMusicVolume(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
            manager.SetSFXVolume(PlayerPrefs.GetFloat(SfxVolumeKey, 1f));
        }

        private void RefreshAllIndicators()
        {
            RefreshIndicator(musicButton, IsEnabled(MusicVolumeKey));
            RefreshIndicator(soundButton, IsEnabled(SfxVolumeKey));
            RefreshIndicator(hapticButton, PlayerPrefs.GetInt(HapticEnabledKey, 1) == 1);
        }

        private static void RefreshIndicator(Button button, bool enabled)
        {
            if (button == null)
                return;

            Transform indicator = button.transform.Find("OnOff");
            if (indicator != null)
                indicator.gameObject.SetActive(enabled);
        }
        private static void RestartGame()
        {
            LoadSceneController.ReloadCurrentScene();
        }

        private void GoHome()
        {
            CloseSettings();

            if (SceneManager.GetActiveScene().name != HomeSceneName)
                LoadSceneController.Load(HomeSceneName);
        }

        private static void StartGame()
        {
            LoadSceneController.Load(GameplaySceneName);
        }
    }
}
