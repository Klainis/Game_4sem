using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Панель производства юнитов. Теперь использует заранее настроенные кнопки вместо динамического создания.
/// </summary>
public class UnitProductionPanel : BaseUIPanel
{
    public static UnitProductionPanel Instance { get; private set; }
    public static event Action ProductionPanelOpened;

    [Header("UI Components")]
    [SerializeField] private UIButton[] unitButtons; // Заранее настроенные кнопки

    [Header("Layout Settings")]  
    [SerializeField] private Vector2 panelSize = new Vector2(250, 450);
    [SerializeField] private GridLayoutGroup layoutGroup;

    private ProductionBuilding currentBuilding;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        
        // Автоматический поиск кнопок если не заданы
        if (unitButtons == null || unitButtons.Length == 0)
        {
            unitButtons = GetComponentsInChildren<UIButton>();
        }
        
        SetupButtons();
    }

    protected override void SetupPanel()
    {
        base.SetupPanel();
        
        // Настраиваем позицию панели справа
        var rectTransform = panelRoot.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(1, 0.5f);
            rectTransform.anchorMax = new Vector2(1, 0.5f);
            rectTransform.pivot = new Vector2(1, 0.5f);
            rectTransform.anchoredPosition = new Vector2(-20, 0);
            rectTransform.sizeDelta = panelSize;
        }

        // Настраиваем layout если есть
        if (layoutGroup == null)
            layoutGroup = panelRoot.GetComponent<GridLayoutGroup>();
    }

    /// <summary>
    /// Настраивает подписки на кнопки
    /// </summary>
    private void SetupButtons()
    {
        for (int i = 0; i < unitButtons.Length; i++)
        {
            if (unitButtons[i] != null)
            {
                unitButtons[i].OnButtonClicked += OnUnitButtonClicked;
                unitButtons[i].Reset(); // Скрываем все кнопки изначально
            }
        }
    }

    /// <summary>
    /// Переключает показ панели для здания
    /// </summary>
    public void Toggle(ProductionBuilding building)
    {
        if (IsOpen && currentBuilding == building)
        {
            Hide();
            return;
        }
        Open(building);
    }

    /// <summary>
    /// Открывает панель для указанного здания
    /// </summary>
    void Open(ProductionBuilding building)
    {
        if (building == null)
        {
            Debug.LogError("[UnitProductionPanel] Trying to open panel with null building!");
            return;
        }
        
        currentBuilding = building;
        Debug.Log($"[UnitProductionPanel] Opening panel for building: {building.name}");
        RefreshButtons();
        
        // Используем унифицированный метод показа
        Show();
        
        // Уведомляем о том, что панель производства открылась
        ProductionPanelOpened?.Invoke();
    }

    protected override void OnShow()
    {
        base.OnShow();
        RefreshButtons();
    }

    protected override void OnHide()
    {
        base.OnHide();
        currentBuilding = null;
    }

    /// <summary>
    /// Обновляет отображение кнопок юнитов
    /// </summary>
    void RefreshButtons()
    {
        if (currentBuilding == null) 
        {
            Debug.LogWarning("[UnitProductionPanel] RefreshButtons called with null currentBuilding");
            return;
        }
        
        if (unitButtons == null || unitButtons.Length == 0)
        {
            Debug.LogWarning("[UnitProductionPanel] unitButtons array is null or empty. Auto-finding buttons...");
            unitButtons = GetComponentsInChildren<UIButton>();
        }
        
        if (unitButtons == null || unitButtons.Length == 0)
        {
            Debug.LogError("[UnitProductionPanel] No UIButton components found in children!");
            return;
        }
        
        int unitCount = currentBuilding.units.Length;
        int availableGold = ResourceManager.Instance ? ResourceManager.Instance.Gold : 0;

        // Настраиваем кнопки для доступных юнитов
        for (int i = 0; i < unitButtons.Length; i++)
        {
            if (unitButtons[i] == null) continue;

            if (i < unitCount)
            {
                var unit = currentBuilding.units[i];
                unitButtons[i].SetData(unit, i);
                unitButtons[i].gameObject.SetActive(true);
                unitButtons[i].UpdateAffordability(availableGold);
            }
            else
            {
                unitButtons[i].Reset();
            }
        }
    }

    /// <summary>
    /// Обработчик клика по кнопке юнита
    /// </summary>
    private void OnUnitButtonClicked(int unitIndex)
    {
        if (currentBuilding != null)
        {
            Debug.Log($"[UnitProductionPanel] Producing unit at index {unitIndex}");
            currentBuilding.Produce(unitIndex);
            
            // Обновляем кнопки после покупки
            RefreshButtons();
        }
    }

    /// <summary>
    /// Обработчик открытия другой панели - закрываем эту панель
    /// </summary>
    protected override void OnOtherPanelOpened(BaseUIPanel otherPanel)
    {
        // Если открылась другая панель (кроме этой), закрываем себя
        if (otherPanel != this)
        {
            Hide();
        }
    }

    /// <summary>
    /// Отписываемся от событий кнопок при уничтожении
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        
        for (int i = 0; i < unitButtons.Length; i++)
        {
            if (unitButtons[i] != null)
            {
                unitButtons[i].OnButtonClicked -= OnUnitButtonClicked;
            }
        }
    }
}
