using System.IO;
using DuyDZ.MergeFood.Test;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class GameHUDSceneBuilder
{
    private const string SpriteFolder = "Assets/Example/Generated/UI";

    [MenuItem("Tools/FruitMerge/Create Scene GameHUD")]
    public static void CreateSceneGameHUD()
    {
        Sprite whiteSprite = GetOrCreateSprite("White", Color.white);
        Sprite blueSprite = GetOrCreateSprite("PanelBlue", new Color(0.85f, 0.96f, 1f, 1f));
        Sprite yellowSprite = GetOrCreateSprite("PanelYellow", new Color(1f, 0.86f, 0.26f, 1f));

        DeleteIfExists("GameHUD");

        GameObject hudObject = new GameObject("GameHUD");
        Canvas canvas = hudObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = hudObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        hudObject.AddComponent<GraphicRaycaster>();
        GameUIManager uiManager = hudObject.AddComponent<GameUIManager>();

        EnsureEventSystem();

        Button settingsButton = CreateButton(hudObject.transform, "SettingsButton", new Vector2(92f, 92f), "\u2699", 48f, whiteSprite);
        SetAnchor(settingsButton.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0.5f, 0.5f), new Vector2(92f, -118f));

        TMP_Text scoreText = CreateText(hudObject.transform, "ScoreText", new Vector2(360f, 120f), new Vector2(0f, 700f), "0", 100f);
        scoreText.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
        scoreText.color = new Color(0.03f, 0.13f, 0.18f, 1f);

        RectTransform nextPanel = CreatePanel(hudObject.transform, "NextPanel", new Vector2(280f, 88f), blueSprite);
        SetAnchor(nextPanel, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-92f, -116f));
        CreateText(nextPanel, "NextLabel", new Vector2(118f, 48f), new Vector2(-58f, 0f), "NEXT", 28f);
        Image nextFruitImage = CreateImage(nextPanel, "NextFruitImage", new Vector2(70f, 70f), new Vector2(74f, 0f), null);

        RectTransform settingsPanel = CreatePanel(hudObject.transform, "SettingsPanel", new Vector2(360f, 220f), blueSprite);
        SetAnchor(settingsPanel, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(36f, -208f));
        settingsPanel.gameObject.SetActive(false);

        Button soundButton = CreateButton(settingsPanel, "SoundButton", new Vector2(300f, 72f), "", 28f, whiteSprite);
        soundButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 48f);
        TMP_Text soundText = soundButton.GetComponentInChildren<TMP_Text>();

        Button exitButton = CreateButton(settingsPanel, "ExitButton", new Vector2(300f, 72f), "EXIT", 30f, whiteSprite);
        exitButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -48f);

        TMP_Text bestScoreText = CreateBestScoreWorldPanel(whiteSprite, yellowSprite);
        AssignReferences(uiManager, scoreText, bestScoreText, soundText, nextFruitImage, settingsPanel.gameObject, settingsButton, soundButton, exitButton);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Selection.activeGameObject = hudObject;
        Debug.Log("Created editable GameHUD in the scene.");
    }

    private static TMP_Text CreateBestScoreWorldPanel(Sprite whiteSprite, Sprite yellowSprite)
    {
        GameObject cloudObject = GameObject.Find("Layer 30 copy 4 1");
        if (cloudObject == null)
        {
            Debug.LogWarning("Cloud object 'Layer 30 copy 4 1' was not found. Best score world panel was not created.");
            return null;
        }

        Transform oldPanel = cloudObject.transform.Find("BestScoreWorldPanel");
        if (oldPanel != null)
            Object.DestroyImmediate(oldPanel.gameObject);

        GameObject panelObject = new GameObject("BestScoreWorldPanel");
        panelObject.transform.SetParent(cloudObject.transform, false);
        panelObject.transform.localPosition = new Vector3(0f, 0.9f, 0.08f);

        GameObject borderObject = new GameObject("Border");
        borderObject.transform.SetParent(panelObject.transform, false);
        borderObject.transform.localPosition = new Vector3(0.04f, -0.04f, 0.01f);

        SpriteRenderer borderRenderer = borderObject.AddComponent<SpriteRenderer>();
        borderRenderer.sprite = whiteSprite;
        borderRenderer.drawMode = SpriteDrawMode.Sliced;
        borderRenderer.size = new Vector2(1.46f, 0.54f);
        borderRenderer.sortingOrder = -3;
        borderRenderer.color = new Color(0.02f, 0.08f, 0.1f, 1f);

        SpriteRenderer panelRenderer = panelObject.AddComponent<SpriteRenderer>();
        panelRenderer.sprite = yellowSprite;
        panelRenderer.drawMode = SpriteDrawMode.Sliced;
        panelRenderer.size = new Vector2(2f, 1f);
        panelRenderer.sortingOrder = -2;

        TMP_Text labelText = CreateWorldText(panelObject.transform, "BestLabel", new Vector3(-0.4f, 0f, -0.01f), new Vector2(2f, 1.8f), "BEST", 3f);
        labelText.color = new Color(0.35f, 0.22f, 0.02f, 1f);

        TMP_Text scoreText = CreateWorldText(panelObject.transform, "BestScoreText", new Vector3(0.4f, 0f, -0.01f), new Vector2(1f, 1.5f), "0", 4f);
        scoreText.color = new Color(0.03f, 0.07f, 0.08f, 1f);
        return scoreText;
    }

    private static void AssignReferences(
        GameUIManager manager,
        TMP_Text scoreText,
        TMP_Text bestScoreText,
        TMP_Text soundText,
        Image nextFruitImage,
        GameObject settingsPanel,
        Button settingsButton,
        Button soundButton,
        Button exitButton)
    {
        SerializedObject serializedObject = new SerializedObject(manager);
        serializedObject.FindProperty("scoreText").objectReferenceValue = scoreText;
        serializedObject.FindProperty("bestScoreText").objectReferenceValue = bestScoreText;
        serializedObject.FindProperty("soundText").objectReferenceValue = soundText;
        serializedObject.FindProperty("nextFruitImage").objectReferenceValue = nextFruitImage;
        serializedObject.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
        serializedObject.FindProperty("settingsButton").objectReferenceValue = settingsButton;
        serializedObject.FindProperty("soundButton").objectReferenceValue = soundButton;
        serializedObject.FindProperty("exitButton").objectReferenceValue = exitButton;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static RectTransform CreatePanel(Transform parent, string name, Vector2 size, Sprite sprite)
    {
        Image image = CreateImage(parent, name, size, Vector2.zero, sprite);
        image.type = Image.Type.Sliced;
        Outline outline = image.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.02f, 0.08f, 0.1f, 1f);
        outline.effectDistance = new Vector2(6f, -6f);
        return image.rectTransform;
    }

    private static Image CreateImage(Transform parent, string name, Vector2 size, Vector2 anchoredPosition, Sprite sprite)
    {
        GameObject imageObject = new GameObject(name);
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        image.preserveAspect = sprite == null;

        RectTransform rectTransform = image.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchoredPosition;
        return image;
    }

    private static Button CreateButton(Transform parent, string name, Vector2 size, string label, float fontSize, Sprite sprite)
    {
        RectTransform rectTransform = CreatePanel(parent, name, size, sprite);
        Button button = rectTransform.gameObject.AddComponent<Button>();
        CreateText(rectTransform, "Label", size, Vector2.zero, label, fontSize);
        return button;
    }

    private static TMP_Text CreateText(Transform parent, string name, Vector2 size, Vector2 anchoredPosition, string value, float fontSize)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = fontSize;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Max(12f, fontSize * 0.45f);
        text.fontSizeMax = fontSize;
        text.enableWordWrapping = false;
        text.color = new Color(0.03f, 0.07f, 0.08f, 1f);
        text.raycastTarget = false;

        RectTransform rectTransform = text.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchoredPosition;
        return text;
    }

    private static TMP_Text CreateWorldText(Transform parent, string name, Vector3 localPosition, Vector2 size, string value, float fontSize)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        textObject.transform.localPosition = localPosition;
        TextMeshPro text = textObject.AddComponent<TextMeshPro>();
        text.text = value;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        text.fontSize = fontSize;
        text.enableAutoSizing = true;
        text.fontSizeMin = fontSize * 0.45f;
        text.fontSizeMax = fontSize;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.sortingOrder = -1;
        text.rectTransform.sizeDelta = size;
        return text;
    }

    private static void SetAnchor(RectTransform rectTransform, Vector2 anchor, Vector2 pivot, Vector2 anchoredPosition)
    {
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = anchoredPosition;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    private static void DeleteIfExists(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
            Object.DestroyImmediate(existing);
    }

    private static Sprite GetOrCreateSprite(string name, Color color)
    {
        Directory.CreateDirectory(SpriteFolder);
        string path = $"{SpriteFolder}/{name}.png";
        if (!File.Exists(path))
        {
            Texture2D texture = new Texture2D(8, 8);
            Color[] pixels = new Color[64];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100f;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
