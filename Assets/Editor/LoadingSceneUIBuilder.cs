using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class LoadingSceneUIBuilder
{
    private const string ScenePath = "Assets/Scenes/LoadScene.unity";
    private const string GeneratedKey = "FruitMerge.LoadingSceneUI.Generated.v1";
    private const string CanvasName = "Loading Canvas";

    [InitializeOnLoadMethod]
    private static void BuildOnceAfterImport()
    {
        if (!EditorPrefs.GetBool(GeneratedKey, false))
            EditorApplication.delayCall += TryBuildOpenLoadingScene;
    }

    [MenuItem("Tools/Fruit Merge/Build Loading Scene UI")]
    public static void BuildFromMenu()
    {
        if (EditorSceneManager.GetActiveScene().path != ScenePath)
        {
            Debug.LogWarning("Open Assets/Scenes/LoadScene.unity before building its UI.");
            return;
        }

        BuildAndSave();
    }

    private static void TryBuildOpenLoadingScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode ||
            EditorSceneManager.GetActiveScene().path != ScenePath)
            return;

        if (BuildAndSave())
            EditorPrefs.SetBool(GeneratedKey, true);
    }

    private static bool BuildAndSave()
    {
        LoadSceneController controller = Object.FindFirstObjectByType<LoadSceneController>();
        if (controller == null)
        {
            Debug.LogError("LoadSceneController was not found in LoadScene.");
            return false;
        }

        GameObject existingCanvas = GameObject.Find(CanvasName);
        RectTransform fillRect;
        RawImage fillImage;

        if (existingCanvas == null)
        {
            CreateInterface(out fillRect, out fillImage);
        }
        else
        {
            Transform fill = existingCanvas.transform.Find("Progress Background/Progress Fill");
            if (fill == null)
            {
                Debug.LogError("Loading Canvas exists but Progress Fill was not found.");
                return false;
            }

            fillRect = fill.GetComponent<RectTransform>();
            fillImage = fill.GetComponent<RawImage>();
        }

        SerializedObject serializedController = new SerializedObject(controller);
        serializedController.FindProperty("fillRect").objectReferenceValue = fillRect;
        serializedController.FindProperty("fillImage").objectReferenceValue = fillImage;
        serializedController.FindProperty("progressWidth").floatValue = 974f;
        serializedController.ApplyModifiedPropertiesWithoutUndo();

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Loading UI was created and saved in LoadScene. You can now edit it in the Hierarchy.");
        return true;
    }

    private static void CreateInterface(out RectTransform fillRect, out RawImage fillImage)
    {
        GameObject canvasObject = new GameObject(
            CanvasName,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasObject, "Build Loading Scene UI");

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1125f, 2436f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0f;

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        Vector2 designSize = new Vector2(1125f, 2436f);

        CreateImage(
            "Background",
            canvasRect,
            "Assets/Example/Export/LoadScene/BG.png",
            designSize,
            new Vector2(0.5f, 0.5f));
        CreateImage(
            "Fruits",
            canvasRect,
            "Assets/Example/Export/LoadScene/fruits loading.png",
            designSize,
            new Vector2(0.5f, 0.5f));
        CreateImage(
            "Welcome",
            canvasRect,
            "Assets/Example/Export/LoadScene/Welcome!.png",
            new Vector2(380f, 59f),
            new Vector2(0.5f, 0.16f));

        RectTransform bar = CreateImage(
            "Progress Background",
            canvasRect,
            "Assets/Example/Export/LoadScene/Loading den.png",
            new Vector2(994f, 72f),
            new Vector2(0.5f, 0.105f));

        RectTransform fill = CreateImage(
            "Progress Fill",
            bar,
            "Assets/Example/Export/LoadScene/Demo loading.png",
            new Vector2(0f, 52f),
            new Vector2(0f, 0.5f));
        fill.pivot = new Vector2(0f, 0.5f);
        fill.anchoredPosition = new Vector2(10f, 0f);

        fillRect = fill;
        fillImage = fill.GetComponent<RawImage>();
    }

    private static RectTransform CreateImage(
        string objectName,
        RectTransform parent,
        string texturePath,
        Vector2 size,
        Vector2 anchor)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(RawImage));
        imageObject.layer = LayerMask.NameToLayer("UI");

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        RawImage image = imageObject.GetComponent<RawImage>();
        image.texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
        image.raycastTarget = false;
        return rect;
    }
}
