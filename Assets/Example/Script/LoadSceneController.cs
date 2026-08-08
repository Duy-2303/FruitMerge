using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Drives the loading UI already authored in LoadScene.</summary>
public sealed class LoadSceneController : MonoBehaviour
{
    public const string LoadingSceneName = "LoadScene";

    private static string pendingScene;

    [Header("Scene UI")]
    [SerializeField] private RectTransform fillRect;
    [SerializeField] private RawImage fillImage;
    [SerializeField, Min(0f)] private float progressWidth = 974f;

    [Header("Loading flow")]
    [SerializeField] private string nextScene = "HomeScene";
    [SerializeField, Min(0f)] private float minimumDisplayTime = 1.25f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetTransitionState()
    {
        pendingScene = null;
    }

    /// <summary>Loads a scene through the loading screen.</summary>
    public static void Load(string targetScene)
    {
        if (string.IsNullOrWhiteSpace(targetScene))
        {
            Debug.LogError("Cannot load an empty scene name.");
            return;
        }

        if (targetScene == LoadingSceneName)
        {
            Debug.LogError("LoadScene cannot be used as its own target scene.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(LoadingSceneName))
        {
            Debug.LogError($"'{LoadingSceneName}' is missing or disabled in Build Settings.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            Debug.LogError($"Target scene '{targetScene}' is missing or disabled in Build Settings.");
            return;
        }

        pendingScene = targetScene;
        Time.timeScale = 1f;
        SceneManager.LoadScene(LoadingSceneName, LoadSceneMode.Single);
    }

    public static void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == LoadingSceneName)
            return;

        Load(currentScene.name);
    }

    private IEnumerator Start()
    {
        if (fillRect == null)
            Debug.LogError("Progress Fill is not assigned in LoadSceneController.", this);

        SetProgress(0f);

        string targetScene = string.IsNullOrWhiteSpace(pendingScene)
            ? nextScene
            : pendingScene;
        pendingScene = null;

        if (targetScene == LoadingSceneName)
        {
            Debug.LogError("The loading screen target cannot be LoadScene.", this);
            yield break;
        }

        float startedAt = Time.realtimeSinceStartup;
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        if (operation == null)
        {
            Debug.LogError($"Loading scene could not find '{targetScene}'. Check Build Settings.", this);
            yield break;
        }

        operation.allowSceneActivation = false;
        while (operation.progress < 0.9f || Time.realtimeSinceStartup - startedAt < minimumDisplayTime)
        {
            float loadProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = minimumDisplayTime <= 0f
                ? 1f
                : Mathf.Clamp01((Time.realtimeSinceStartup - startedAt) / minimumDisplayTime);
            SetProgress(Mathf.Min(loadProgress, timeProgress));
            yield return null;
        }

        SetProgress(1f);
        yield return null;
        operation.allowSceneActivation = true;
    }

    private void SetProgress(float progress)
    {
        if (fillRect == null)
            return;

        float clampedProgress = Mathf.Clamp01(progress);
        fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, progressWidth * clampedProgress);
        if (fillImage != null)
            fillImage.uvRect = new Rect(0f, 0f, clampedProgress, 1f);
    }
}
