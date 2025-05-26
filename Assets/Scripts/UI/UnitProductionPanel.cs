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

    readonly List<Button> pooled = new();
    ProductionBuilding    current;

    void Awake()
    {
        Instance = this;
        panelRoot.SetActive(false);

        // Настраиваем позицию панели справа
        var rectTransform = panelRoot.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 0.5f); // Правый край
        rectTransform.anchorMax = new Vector2(1, 0.5f);
        rectTransform.pivot = new Vector2(1, 0.5f);
        rectTransform.anchoredPosition = new Vector2(-10, 0); // Отступ от правого края
        rectTransform.sizeDelta = new Vector2(180, 365); // Уменьшенная ширина панели

        // Добавляем и настраиваем GridLayoutGroup если его нет
        if (!panelRoot.GetComponent<GridLayoutGroup>())
        {
            var layout = panelRoot.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(160, 45); // Уменьшенный размер кнопок
            layout.spacing = new Vector2(5, 5);   // Расстояние между кнопками
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 1; // Одна кнопка в ряду
        }
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
        int count = current.units.Length;

        // 1) создаём/настраиваем нужное число кнопок
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
                // явное добавление в пул
                btn = Instantiate(buttonPrefab, panelRoot.transform);
                btn.gameObject.AddComponent<UnitButton>();  // Добавляем компонент для контроля размера
                pooled.Add(btn);
            }

            int idx = i;  // для лямбды
            var opt = current.units[i];

            // Получаем TMP_Text внутри кнопки
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label == null)
                Debug.LogError("UnitProductionPanel: в buttonPrefab нет TMP_Text!");
            else
                label.text = $"{opt.name} ({opt.cost})";

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => current.Produce(idx));
        }

        // 2) прячем лишние кнопки
        for (int i = count; i < pooled.Count; i++)
        {
            pooled[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // ПКМ сворачивает панель
        if (panelRoot.activeSelf && Input.GetMouseButtonDown(1))
            Hide();
    }
}
