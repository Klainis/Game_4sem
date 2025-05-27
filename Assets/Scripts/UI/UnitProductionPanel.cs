using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitProductionPanel : MonoBehaviour
{
    public static UnitProductionPanel Instance  { get; private set; }
    public static event Action ProductionPanelOpened;

    [Header("UI ссылки")]
    [SerializeField] GameObject panelRoot;   // контейнер панели
    [SerializeField] Button     buttonPrefab; // должен иметь внутри TMP_Text

    [Header("Panel Settings")]
    [SerializeField] private Image backgroundImage; // Фоновое изображение
    [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Цвет фона
    [SerializeField] private Vector2 panelSize = new Vector2(260, 265); // Размер панели
    [SerializeField] private Vector2 buttonSize = new Vector2(210, 50); // Размер кнопок
    [SerializeField] private Vector2 spacing = new Vector2(0, 10); // Расстояние между кнопками
    [SerializeField] private int paddingLeft = 15;
    [SerializeField] private int paddingRight = 15;
    [SerializeField] private int paddingTop = 15;
    [SerializeField] private int paddingBottom = 15;

    readonly List<Button> pooled = new();
    ProductionBuilding    current;
    private GridLayoutGroup layoutGroup;

    private int idx;
    void Awake()
    {
        Instance = this;
        SetupPanel();
    }

    private void SetupPanel()
    {
        panelRoot.SetActive(false);

        // Настраиваем фон
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
            backgroundImage.transform.SetSiblingIndex(0);
        }

        // Настраиваем позицию панели справа
        var rectTransform = panelRoot.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 0.5f);
        rectTransform.anchorMax = new Vector2(1, 0.5f);
        rectTransform.pivot = new Vector2(1, 0.5f);
        rectTransform.anchoredPosition = new Vector2(-20, 0); // Немного больший отступ от края
        rectTransform.sizeDelta = panelSize;

        // Настраиваем layout для кнопок
        layoutGroup = panelRoot.GetComponent<GridLayoutGroup>();
        if (!layoutGroup)
        {
            layoutGroup = panelRoot.AddComponent<GridLayoutGroup>();
        }
        
        layoutGroup.cellSize = buttonSize;
        layoutGroup.spacing = spacing;
        layoutGroup.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layoutGroup.constraintCount = 1;
    }

    /// <summary>
    /// Переключает показ панели для здания b
    /// </summary>
    public void Toggle(ProductionBuilding b)
    {
        if (panelRoot.activeSelf && current == b)
        {
            Hide();
            return;
        }
        Open(b);
    }

    void Open(ProductionBuilding b)
    {
        // Скрываем все остальные панели, если нужно
        ProductionPanelOpened?.Invoke();

        current = b;
        RefreshButtons();
        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
        current = null;
    }

    void RefreshButtons()
    {
        if (current == null) return;
        
        int count = current.units.Length;

        for (int i = 0; i < count; i++)
        {
            Button btn;
            if (i < pooled.Count)
            {
                btn = pooled[i];
                btn.gameObject.SetActive(true);
            }
            else
            {
                btn = Instantiate(buttonPrefab, panelRoot.transform);
                pooled.Add(btn);
            }

            var opt = current.units[i];
            var label = btn.GetComponentInChildren<TMP_Text>();
            label.text = $"{opt.name} ({opt.cost})";

            btn.onClick.RemoveAllListeners();
            int localIndex = i; // Критически важная локальная переменная
            btn.onClick.AddListener(() => {
                if (current != null)
                    current.Produce(localIndex);
            });
        }

        for (int i = count; i < pooled.Count; i++)
            pooled[i].gameObject.SetActive(false);
    }
    void OnButtonClick()
    {
        Debug.Log("ВЫЗВАНО СОЗДАНИЕ ЮНИТА");
        current.Produce(idx);
    }

    void Update()
    {
        // ПКМ сворачивает панель
        if (panelRoot.activeSelf && Input.GetMouseButtonDown(1))
            Hide();
    }
}
