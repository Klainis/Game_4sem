using UnityEngine;

/// <summary>
/// Открывает/закрывает панель построек по клику на главное здание.
/// Теперь работает с унифицированной системой UI-панелей.
/// </summary>
public class BuildingOpener : MonoBehaviour
{
    [Header("UI Панель строительства")]
    [SerializeField] BuildingUI buildingUI;

    void Awake()
    {
        // Автоматический поиск BuildingUI если не задан
        if (buildingUI == null)
            buildingUI = FindObjectOfType<BuildingUI>();
    }

    void OnMouseDown() => Toggle();

    /// <summary>
    /// Переключает показ панели строительства
    /// </summary>
    void Toggle()
    {
        if (buildingUI == null) return;

        buildingUI.Toggle();
        
        Debug.Log($"[BuildingOpener] Toggled building UI. Is open: {buildingUI.IsOpen}");
    }
}
