using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Builds and drives the startup loading screen from the imported artwork.</summary>
public sealed class LoadSceneController : MonoBehaviour
{
    public const string LoadingSceneName = "LoadScene";

    private static string pendingScene;

    [Header("Loading artwork")]
    [SerializeField] private Texture background;
    [SerializeField] private Texture fruits;
    [SerializeField] private Texture welcome;
    [SerializeField] private Texture progressBackground;
    [SerializeField] private Texture progressFill;

    [Header("Loading flow")]
    [SerializeField] private string nextScene = "HomeScene";
    [SerializeField, Min(0f)] private float minimumDisplayTime = 1.25f;

    private RectTransform fillRect;
    private RawImage fillImage;

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
        BuildInterface();

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

    private void BuildInterface()
    {
        var canvasObject = new GameObject("Loading Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1125f, 2436f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        CreateFullScreenImage("Background", canvasRect, background);
        CreateFullScreenImage("Fruits", canvasRect, fruits);

        CreateImage("Welcome", canvasRect, welcome, new Vector2(380f, 59f), new Vector2(0.5f, 0.16f));
        RectTransform bar = CreateImage("Progress Background", canvasRect, progressBackground,
            new Vector2(994f, 72f), new Vector2(0.5f, 0.105f));

        GameObject fillObject = new GameObject("Progress Fill", typeof(RectTransform), typeof(RawImage));
        fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.SetParent(bar, false);
        fillRect.anchorMin = new Vector2(0f, 0.5f);
        fillRect.anchorMax = new Vector2(0f, 0.5f);
        fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.anchoredPosition = new Vector2(10f, 0f);
        fillRect.sizeDelta = new Vector2(0f, 52f);

        fillImage = fillObject.GetComponent<RawImage>();
        fillImage.texture = progressFill;
        fillImage.raycastTarget = false;
        SetProgress(0f);
    }

    private static RectTransform CreateFullScreenImage(string name, RectTransform parent, Texture texture)
    {
        RectTransform rect = CreateImage(name, parent, texture, Vector2.zero, new Vector2(0.5f, 0.5f));
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static RectTransform CreateImage(string name, RectTransform parent, Texture texture, Vector2 size, Vector2 anchor)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(RawImage));
        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        RawImage image = imageObject.GetComponent<RawImage>();
        image.texture = texture;
        image.raycastTarget = false;
        return rect;
    }

    private void SetProgress(float progress)
    {
        if (fillRect == null)
            return;

        float clampedProgress = Mathf.Clamp01(progress);
        fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 974f * clampedProgress);
        if (fillImage != null)
            fillImage.uvRect = new Rect(0f, 0f, clampedProgress, 1f);
    }
}
