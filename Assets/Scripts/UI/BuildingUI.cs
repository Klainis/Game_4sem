using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuildingOption
{
    public string name;
    public GameObject prefab;          // Основной префаб здания
    public GameObject ghostPrefab;     // Ghost-префаб (опционально)
    public int cost;
    public Sprite icon; // Иконка здания
}

/// <summary>
/// UI-панель выбора зданий. Теперь использует заранее настроенные кнопки.
/// </summary>
public class BuildingUI : BaseUIPanel
{
    [Header("References")]
    public BuildingPlacementManager placementManager;
    public BuildingOption[] buildingOptions;
    public GameObject floatingTextPrefab;

    [Header("UI Components")]
    [SerializeField] private UIButton[] buildingButtons; // Заранее настроенные кнопки
    [SerializeField] private GridLayoutGroup buttonLayout;

    protected override void Awake()
    {
        base.Awake();
        
        // Автоматический поиск кнопок если не заданы
        if (buildingButtons == null || buildingButtons.Length == 0)
        {
            buildingButtons = GetComponentsInChildren<UIButton>();
        }
        
        SetupBuildingButtons();
    }

    protected override void SetupPanel()
    {
        base.SetupPanel();
        
        // Настраиваем layout для кнопок, если он есть
        if (buttonLayout != null)
        {
            buttonLayout.cellSize = new Vector2(160, 45);
            buttonLayout.spacing = new Vector2(10, 10);
            buttonLayout.padding = new RectOffset(10, 10, 10, 10);
            buttonLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            buttonLayout.constraintCount = 3;
        }
    }

    /// <summary>
    /// Настраивает кнопки зданий
    /// </summary>
    private void SetupBuildingButtons()
    {
        for (int i = 0; i < buildingButtons.Length; i++)
        {
            if (buildingButtons[i] != null)
            {
                buildingButtons[i].OnButtonClicked += OnBuildingButtonClicked;
                
                // Настраиваем кнопку с данными здания если есть
                if (i < buildingOptions.Length)
                {
                    var option = buildingOptions[i];
                    buildingButtons[i].Activate(option.name, option.cost, option.icon, i);
                    Debug.Log($"[BuildingUI] Activated button {i}: {option.name}");
                }
                else
                {
                    buildingButtons[i].Reset();
                    Debug.Log($"[BuildingUI] Reset button {i}");
                }
            }
        }
    }

    /// <summary>
    /// Обработчик клика по кнопке здания
    /// </summary>
    private void OnBuildingButtonClicked(int buildingIndex)
    {
        SelectBuilding(buildingIndex);
        // Панель остается открытой для быстрого выбора следующего здания
    }

    /// <summary>
    /// Выбирает здание для постройки
    /// </summary>
    public void SelectBuilding(int index)
    {
        if (placementManager != null && index >= 0 && index < buildingOptions.Length)
        {
            var option = buildingOptions[index];
            placementManager.BeginPlacement(option.prefab, option.ghostPrefab, option.cost);
            Debug.Log($"[BuildingUI] Selected building: {option.name}");
        }
    }

    /// <summary>
    /// Обновляет интерактивность кнопок в зависимости от доступного золота
    /// </summary>
    protected override void OnShow()
    {
        base.OnShow();
        Debug.Log($"[BuildingUI] OnShow called. BuildingOptions count: {buildingOptions?.Length ?? 0}, Buttons count: {buildingButtons?.Length ?? 0}");
        SetupBuildingButtons(); // Переустанавливаем кнопки при каждом открытии
        UpdateButtonAffordability();
    }

    /// <summary>
    /// Обновляет доступность кнопок
    /// </summary>
    private void UpdateButtonAffordability()
    {
        int availableGold = ResourceManager.Instance ? ResourceManager.Instance.Gold : 0;
        
        for (int i = 0; i < buildingButtons.Length && i < buildingOptions.Length; i++)
        {
            if (buildingButtons[i] != null)
            {
                buildingButtons[i].UpdateAffordability(availableGold);
            }
        }
    }

    /// <summary>
    /// Обработчик открытия другой панели
    /// </summary>
    protected override void OnOtherPanelOpened(BaseUIPanel otherPanel)
    {
        // Если открылась панель производства юнитов, закрываем панель строительства
        if (otherPanel is UnitProductionPanel)
        {
            Hide();
        }
    }

    /// <summary>
    /// Отписываемся от событий кнопок при отключении
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        
        for (int i = 0; i < buildingButtons.Length; i++)
        {
            if (buildingButtons[i] != null)
            {
                buildingButtons[i].OnButtonClicked -= OnBuildingButtonClicked;
            }
        }
    }

    // Публичные методы для совместимости с существующим кодом
    public void ShowUI() => Show();
    public void HideUI() => Hide();
}
