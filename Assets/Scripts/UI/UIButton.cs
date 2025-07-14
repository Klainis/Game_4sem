using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Унифицированный компонент для кнопок UI с автоматической настройкой и связыванием данных.
/// Поддерживает отображение названия, стоимости, иконки и обработку клика.
/// </summary>
[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    [Header("Display Settings")]
    [SerializeField] private string titleFormat = "{0}";
    [SerializeField] private string costFormat = "({0})";
    [SerializeField] private bool hideCostIfZero = true;
    [SerializeField] private bool hideIconIfNull = true;

    [Header("Button Data")]
    [SerializeField] private string buttonTitle = "";
    [SerializeField] private int buttonCost = 0;
    [SerializeField] private Sprite buttonIcon = null;
    [SerializeField] private int buttonIndex = 0;

    // События для обработки кликов
    public event Action<int> OnButtonClicked;
    public event Action<UIButton> OnButtonSelected;

    private bool isInitialized = false;

    void Awake()
    {
        // Автоматический поиск компонентов если не заданы
        FindComponents();
        
        // Настройка кнопки
        SetupButton();
        
        isInitialized = true;
        
        // Применяем начальные данные
        RefreshDisplay();
    }

    /// <summary>
    /// Автоматически находит необходимые компоненты
    /// </summary>
    private void FindComponents()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (titleText == null)
            titleText = GetComponentInChildren<TMP_Text>();

        // Ищем все текстовые компоненты и определяем какой для чего
        var allTexts = GetComponentsInChildren<TMP_Text>();
        if (allTexts.Length >= 2)
        {
            titleText = allTexts[0];
            costText = allTexts[1];
        }

        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>();

        // Убедимся, что кнопка растягивается на всю ячейку
        var rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }

    /// <summary>
    /// Настраивает кнопку и подписывается на клик
    /// </summary>
    private void SetupButton()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(HandleClick);
        }
    }

    /// <summary>
    /// Устанавливает данные кнопки
    /// </summary>
    public void SetData(string title, int cost, Sprite icon, int index)
    {
        buttonTitle = title;
        buttonCost = cost;
        buttonIcon = icon;
        buttonIndex = index;

        if (isInitialized)
            RefreshDisplay();
    }

    /// <summary>
    /// Устанавливает данные кнопки из BuildingOption
    /// </summary>
    public void SetData(BuildingOption option, int index)
    {
        SetData(option.name, option.cost, option.icon, index);
    }

    /// <summary>
    /// Устанавливает данные кнопки из UnitOption
    /// </summary>
    public void SetData(ProductionBuilding.UnitOption option, int index)
    {
        SetData(option.name, option.cost, null, index); // У юнитов пока нет иконок
    }

    /// <summary>
    /// Обновляет отображение кнопки
    /// </summary>
    public void RefreshDisplay()
    {
        UpdateTitle();
        UpdateCost();
        UpdateIcon();
    }

    /// <summary>
    /// Обновляет заголовок
    /// </summary>
    private void UpdateTitle()
    {
        if (titleText != null)
        {
            titleText.text = string.Format(titleFormat, buttonTitle);
        }
    }

    /// <summary>
    /// Обновляет отображение стоимости
    /// </summary>
    private void UpdateCost()
    {
        if (costText != null)
        {
            if (hideCostIfZero && buttonCost == 0)
            {
                costText.text = "";
            }
            else
            {
                costText.text = string.Format(costFormat, buttonCost);
            }
        }
    }

    /// <summary>
    /// Обновляет иконку
    /// </summary>
    private void UpdateIcon()
    {
        if (iconImage != null)
        {
            iconImage.sprite = buttonIcon;
            
            if (hideIconIfNull && buttonIcon == null)
            {
                iconImage.gameObject.SetActive(false);
            }
            else
            {
                iconImage.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Обработчик клика по кнопке
    /// </summary>
    private void HandleClick()
    {
        OnButtonClicked?.Invoke(buttonIndex);
        OnButtonSelected?.Invoke(this);
        
        Debug.Log($"[UIButton] Clicked: {buttonTitle} (Index: {buttonIndex}, Cost: {buttonCost})");
    }

    /// <summary>
    /// Устанавливает интерактивность кнопки
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (button != null)
            button.interactable = interactable;
    }

    /// <summary>
    /// Проверяет, достаточно ли ресурсов для покупки
    /// </summary>
    public bool CanAfford(int availableGold)
    {
        return availableGold >= buttonCost;
    }

    /// <summary>
    /// Обновляет интерактивность в зависимости от доступных ресурсов
    /// </summary>
    public void UpdateAffordability(int availableGold)
    {
        SetInteractable(CanAfford(availableGold));
        
        // Можно добавить визуальную индикацию недоступности
        if (costText != null)
        {
            costText.color = CanAfford(availableGold) ? Color.white : Color.red;
        }
    }

    /// <summary>
    /// Сбрасывает кнопку в исходное состояние
    /// </summary>
    public void Reset()
    {
        SetData("", 0, null, 0);
        SetInteractable(true);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Активирует кнопку с данными
    /// </summary>
    public void Activate(string title, int cost, Sprite icon, int index)
    {
        SetData(title, cost, icon, index);
        gameObject.SetActive(true);
        SetInteractable(true);
    }

    #region Public Properties
    public string Title => buttonTitle;
    public int Cost => buttonCost;
    public Sprite Icon => buttonIcon;
    public int Index => buttonIndex;
    public bool IsInteractable => button != null && button.interactable;
    #endregion

    #region Editor Helpers
    #if UNITY_EDITOR
    [ContextMenu("Find All Components")]
    void FindAllComponents()
    {
        FindComponents();
        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Test Button")]
    void TestButton()
    {
        SetData("Test Item", 100, null, 0);
    }
    #endif
    #endregion
} 