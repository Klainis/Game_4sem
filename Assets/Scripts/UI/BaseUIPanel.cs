using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Базовый класс для всех UI-панелей в игре.
/// Обеспечивает унифицированное управление показом/скрытием панелей.
/// </summary>
public abstract class BaseUIPanel : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] protected GameObject panelRoot;
    [SerializeField] protected bool hideOnStart = true;
    [SerializeField] protected bool closeOnRightClick = true;

    [Header("Background Settings")]
    [SerializeField] protected Image backgroundImage;
    [SerializeField] protected Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);

    // События для координации между панелями
    public static event Action<BaseUIPanel> PanelOpened;
    public static event Action<BaseUIPanel> PanelClosed;

    protected bool isOpen = false;

    protected virtual void Awake()
    {
        // Автоматический поиск panelRoot если не задан
        if (panelRoot == null)
            panelRoot = gameObject;

        // Автоматический поиск backgroundImage если не задан
        if (backgroundImage == null)
            backgroundImage = GetComponentInChildren<Image>();

        SetupPanel();
        
        // Скрываем панель сразу в Awake если hideOnStart = true
        if (hideOnStart)
        {
            // Принудительно активируем панель, чтобы потом корректно её скрыть
            if (panelRoot != null && !panelRoot.activeSelf)
            {
                panelRoot.SetActive(true);
            }
            Hide();
        }
    }

    protected virtual void Start()
    {
        // Start больше ничего не делает - всё перенесено в Awake
    }

    protected virtual void Update()
    {
        // ПКМ закрывает панель если опция включена
        if (closeOnRightClick && isOpen && Input.GetMouseButtonDown(1))
        {
            Hide();
        }
    }

    /// <summary>
    /// Настройка панели. Переопределяется в наследниках для специфичных настроек.
    /// </summary>
    protected virtual void SetupPanel()
    {
        SetupBackground();
    }

    /// <summary>
    /// Настройка фона панели
    /// </summary>
    protected virtual void SetupBackground()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
            // Убедимся, что фон позади всех элементов
            backgroundImage.transform.SetSiblingIndex(0);
        }
    }

    /// <summary>
    /// Показать панель
    /// </summary>
    public virtual void Show()
    {
        if (isOpen) return;

        // Убеждаемся, что сам GameObject активен
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
        
        isOpen = true;
        
        // Уведомляем другие панели о том, что эта панель открылась
        PanelOpened?.Invoke(this);
        
        Debug.Log($"[BaseUIPanel] Showing panel: {gameObject.name}");
        OnShow();
    }

    /// <summary>
    /// Скрыть панель
    /// </summary>
    public virtual void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
        
        isOpen = false;
        
        PanelClosed?.Invoke(this);
        
        Debug.Log($"[BaseUIPanel] Hiding panel: {gameObject.name}");
        OnHide();
    }

    /// <summary>
    /// Переключить видимость панели
    /// </summary>
    public virtual void Toggle()
    {
        if (IsOpen)
            Hide();
        else
            Show();
    }

    /// <summary>
    /// Проверка, открыта ли панель
    /// </summary>
    public bool IsOpen 
    { 
        get 
        { 
            // Проверяем реальное состояние panelRoot
            if (panelRoot != null)
                return panelRoot.activeInHierarchy;
            return isOpen;
        } 
    }

    /// <summary>
    /// Вызывается при показе панели. Переопределяется в наследниках.
    /// </summary>
    protected virtual void OnShow() 
    {
        Debug.Log($"[{GetType().Name}] Panel shown");
    }

    /// <summary>
    /// Вызывается при скрытии панели. Переопределяется в наследниках.
    /// </summary>
    protected virtual void OnHide() 
    {
        Debug.Log($"[{GetType().Name}] Panel hidden");
    }

    /// <summary>
    /// Закрывает все другие панели кроме этой
    /// </summary>
    protected void CloseOtherPanels()
    {
        var allPanels = FindObjectsOfType<BaseUIPanel>();
        foreach (var panel in allPanels)
        {
            if (panel != this && panel.IsOpen)
            {
                panel.Hide();
            }
        }
    }

    /// <summary>
    /// Подписывается на события других панелей для автоматического закрытия
    /// </summary>
    protected virtual void OnEnable()
    {
        PanelOpened += OnOtherPanelOpened;
    }

    /// <summary>
    /// Отписывается от событий
    /// </summary>
    protected virtual void OnDisable()
    {
        PanelOpened -= OnOtherPanelOpened;
    }

    /// <summary>
    /// Обработчик открытия другой панели. По умолчанию ничего не делает.
    /// Переопределяется в наследниках для специфичного поведения.
    /// </summary>
    protected virtual void OnOtherPanelOpened(BaseUIPanel otherPanel)
    {
        // По умолчанию ничего не делаем
        // Наследники могут переопределить это поведение
    }

    #region Editor Helpers
    #if UNITY_EDITOR
    [ContextMenu("Find Panel Root")]
    void FindPanelRoot()
    {
        if (panelRoot == null)
        {
            panelRoot = gameObject;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    [ContextMenu("Find Background Image")]
    void FindBackgroundImage()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponentInChildren<Image>();
            if (backgroundImage != null)
                UnityEditor.EditorUtility.SetDirty(this);
        }
    }
    #endif
    #endregion
} 