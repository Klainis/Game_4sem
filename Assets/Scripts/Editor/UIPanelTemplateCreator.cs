#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Утилита для создания шаблонов UI-панелей с предустановленными кнопками
/// </summary>
public class UIPanelTemplateCreator : EditorWindow
{
    private string panelName = "NewUIPanel";
    private int buttonCount = 5;
    private Vector2 panelSize = new Vector2(300, 400);
    private Vector2 buttonSize = new Vector2(160, 45);
    private Vector2 spacing = new Vector2(10, 10);
    private RectOffset padding;
    private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    
    private enum PanelType
    {
        BuildingPanel,
        UnitProductionPanel,
        GenericPanel
    }
    private PanelType panelType = PanelType.GenericPanel;

    [MenuItem("Tools/UI System/Create UI Panel Template")]
    public static void ShowWindow()
    {
        var window = GetWindow<UIPanelTemplateCreator>();
        Debug.Log("UI Panel Template Creator window opened");
        window.titleContent = new GUIContent("UI Panel Template Creator");
        window.Show();
    }

    void OnEnable()
    {
        // Инициализируем padding здесь, чтобы избежать ошибки сериализации
        if (padding == null)
            padding = new RectOffset(10, 10, 10, 10);
    }

    void OnGUI()
    {
        // Дополнительная проверка на null на случай потери состояния при перекомпиляции
        if (padding == null)
        {
            padding = new RectOffset(10, 10, 10, 10);
        }
        
        GUILayout.Label("UI Panel Template Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        panelName = EditorGUILayout.TextField("Panel Name", panelName);
        panelType = (PanelType)EditorGUILayout.EnumPopup("Panel Type", panelType);
        buttonCount = EditorGUILayout.IntSlider("Button Count", buttonCount, 1, 10);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Panel Settings", EditorStyles.boldLabel);
        panelSize = EditorGUILayout.Vector2Field("Panel Size", panelSize);
        backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Button Settings", EditorStyles.boldLabel);
        buttonSize = EditorGUILayout.Vector2Field("Button Size", buttonSize);
        spacing = EditorGUILayout.Vector2Field("Spacing", spacing);
        
        EditorGUILayout.LabelField("Padding");
        EditorGUI.indentLevel++;
        padding.left = EditorGUILayout.IntField("Left", padding.left);
        padding.right = EditorGUILayout.IntField("Right", padding.right);
        padding.top = EditorGUILayout.IntField("Top", padding.top);
        padding.bottom = EditorGUILayout.IntField("Bottom", padding.bottom);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Panel Template"))
        {
            CreatePanelTemplate();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This creates a UI panel prefab with:\n" +
            "• Background Image\n" +
            "• Grid Layout Group\n" +
            "• Pre-configured UIButton components\n" +
            "• Appropriate panel script component",
            MessageType.Info
        );
    }

    void CreatePanelTemplate()
    {
        // Создаем корневой объект панели
        GameObject panel = new GameObject(panelName);
        
        // Добавляем RectTransform
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.sizeDelta = panelSize;
        
        // Добавляем Canvas Group для управления интерактивностью
        panel.AddComponent<CanvasGroup>();
        
        // Создаем фоновое изображение
        GameObject background = new GameObject("Background");
        background.transform.SetParent(panel.transform);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = backgroundColor;
        
        // Создаем контейнер для кнопок
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(panel.transform);
        
        RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.sizeDelta = Vector2.zero;
        containerRect.anchoredPosition = Vector2.zero;
        
        // Добавляем Grid Layout Group
        GridLayoutGroup layout = buttonContainer.AddComponent<GridLayoutGroup>();
        layout.cellSize = buttonSize;
        layout.spacing = spacing;
        layout.padding = padding;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 1;
        
        // Создаем кнопки
        for (int i = 0; i < buttonCount; i++)
        {
            CreateUIButton(buttonContainer.transform, i);
        }
        
        // Добавляем соответствующий компонент панели
        switch (panelType)
        {
            case PanelType.BuildingPanel:
                var buildingUI = panel.AddComponent<BuildingUI>();
                SetupBuildingUI(buildingUI, panel, bgImage, layout);
                break;
                
            case PanelType.UnitProductionPanel:
                var unitPanel = panel.AddComponent<UnitProductionPanel>();
                SetupUnitProductionPanel(unitPanel, panel, bgImage, layout);
                break;
                
            case PanelType.GenericPanel:
                var basePanel = panel.AddComponent<BaseUIPanel>();
                SetupBasePanel(basePanel, panel, bgImage);
                break;
        }
        
        // Сохраняем как префаб
        string path = $"Assets/Prefabs/UI/{panelName}.prefab";
        
        // Создаем папку если её нет
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(panel, path);
        
        if (prefab != null)
        {
            EditorUtility.DisplayDialog("Success", 
                $"UI Panel template created successfully!\nPath: {path}", "OK");
            
            EditorGUIUtility.PingObject(prefab);
            Selection.activeObject = prefab;
        }
        
        // Удаляем временный объект из сцены
        DestroyImmediate(panel);
    }

    void CreateUIButton(Transform parent, int index)
    {
        GameObject button = new GameObject($"Button_{index}");
        button.transform.SetParent(parent);
        
        // RectTransform
        RectTransform buttonRect = button.AddComponent<RectTransform>();
        buttonRect.anchorMin = Vector2.zero;
        buttonRect.anchorMax = Vector2.one;
        buttonRect.sizeDelta = Vector2.zero;
        
        // Button component
        Button buttonComp = button.AddComponent<Button>();
        
        // Image для кнопки
        Image buttonImage = button.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        buttonComp.targetGraphic = buttonImage;
        
        // Создаем текст заголовка
        GameObject titleText = new GameObject("TitleText");
        titleText.transform.SetParent(button.transform);
        
        RectTransform titleRect = titleText.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.5f);
        titleRect.anchorMax = new Vector2(0.7f, 1);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI titleTMP = titleText.AddComponent<TextMeshProUGUI>();
        titleTMP.text = $"Item {index + 1}";
        titleTMP.fontSize = 14;
        titleTMP.alignment = TextAlignmentOptions.Left;
        titleTMP.margin = new Vector4(5, 0, 0, 0);
        
        // Создаем текст стоимости
        GameObject costText = new GameObject("CostText");
        costText.transform.SetParent(button.transform);
        
        RectTransform costRect = costText.AddComponent<RectTransform>();
        costRect.anchorMin = new Vector2(0.7f, 0.5f);
        costRect.anchorMax = new Vector2(1, 1);
        costRect.sizeDelta = Vector2.zero;
        costRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI costTMP = costText.AddComponent<TextMeshProUGUI>();
        costTMP.text = $"(100)";
        costTMP.fontSize = 12;
        costTMP.alignment = TextAlignmentOptions.Right;
        costTMP.margin = new Vector4(0, 0, 5, 0);
        
        // Добавляем UIButton компонент
        UIButton uiButton = button.AddComponent<UIButton>();
        
        // Устанавливаем ссылки через SerializedObject для корректной работы
        SerializedObject serializedButton = new SerializedObject(uiButton);
        serializedButton.FindProperty("titleText").objectReferenceValue = titleTMP;
        serializedButton.FindProperty("costText").objectReferenceValue = costTMP;
        serializedButton.FindProperty("button").objectReferenceValue = buttonComp;
        serializedButton.FindProperty("buttonTitle").stringValue = $"Item {index + 1}";
        serializedButton.FindProperty("buttonCost").intValue = 100;
        serializedButton.FindProperty("buttonIndex").intValue = index;
        serializedButton.ApplyModifiedProperties();
    }

    void SetupBuildingUI(BuildingUI buildingUI, GameObject panel, Image bgImage, GridLayoutGroup layout)
    {
        SerializedObject serializedPanel = new SerializedObject(buildingUI);
        serializedPanel.FindProperty("panelRoot").objectReferenceValue = panel;
        serializedPanel.FindProperty("backgroundImage").objectReferenceValue = bgImage;
        serializedPanel.FindProperty("buttonLayout").objectReferenceValue = layout;
        serializedPanel.FindProperty("buildingButtons").arraySize = buttonCount;
        
        var buttons = panel.GetComponentsInChildren<UIButton>();
        for (int i = 0; i < buttons.Length && i < buttonCount; i++)
        {
            serializedPanel.FindProperty("buildingButtons").GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];
        }
        
        serializedPanel.ApplyModifiedProperties();
    }

    void SetupUnitProductionPanel(UnitProductionPanel unitPanel, GameObject panel, Image bgImage, GridLayoutGroup layout)
    {
        SerializedObject serializedPanel = new SerializedObject(unitPanel);
        serializedPanel.FindProperty("panelRoot").objectReferenceValue = panel;
        serializedPanel.FindProperty("backgroundImage").objectReferenceValue = bgImage;
        serializedPanel.FindProperty("layoutGroup").objectReferenceValue = layout;
        serializedPanel.FindProperty("unitButtons").arraySize = buttonCount;
        
        var buttons = panel.GetComponentsInChildren<UIButton>();
        for (int i = 0; i < buttons.Length && i < buttonCount; i++)
        {
            serializedPanel.FindProperty("unitButtons").GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];
        }
        
        serializedPanel.ApplyModifiedProperties();
    }

    void SetupBasePanel(BaseUIPanel basePanel, GameObject panel, Image bgImage)
    {
        SerializedObject serializedPanel = new SerializedObject(basePanel);
        serializedPanel.FindProperty("panelRoot").objectReferenceValue = panel;
        serializedPanel.FindProperty("backgroundImage").objectReferenceValue = bgImage;
        serializedPanel.ApplyModifiedProperties();
    }
}
#endif 