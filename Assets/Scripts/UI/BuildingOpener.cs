using UnityEngine;

/// <summary>
/// Открывает/закрывает панель построек по клику на главное здание.
/// Автоматически закрывается, если открылась панель найма.
/// </summary>
public class BuildingOpener : MonoBehaviour
{
    [Header("UI Панель строительства")]
    [SerializeField] GameObject buildingUIPanel;

    bool isOpen;

    void Awake()
    {
        // Подписка: когда открывается Production-панель, эта скрывается
        UnitProductionPanel.ProductionPanelOpened += ForceClose;
    }

    void OnDestroy()
    {
        UnitProductionPanel.ProductionPanelOpened -= ForceClose;
    }

    void OnMouseDown() => Toggle();

    /*–––– helpers ––––*/
    void Toggle()
    {
        if (!buildingUIPanel) return;

        isOpen = !isOpen;
        buildingUIPanel.SetActive(isOpen);

        // Если открываем себя → закрываем панель найма
        if (isOpen && UnitProductionPanel.Instance != null)
            UnitProductionPanel.Instance.Hide();
    }

    void ForceClose()
    {
        if (!isOpen || !buildingUIPanel) return;
        isOpen = false;
        buildingUIPanel.SetActive(false);
    }
}
