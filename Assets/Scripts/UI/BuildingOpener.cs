using UnityEngine;

public class BuildingOpener : MonoBehaviour
{
    [Header("UI Панель строительства")]
    public GameObject buildingUIPanel; // сюда закидываем сам UI-панель с кнопками
    private bool isPanelOpen = false; // флаг, открыта ли панель
    private void OnMouseDown()
    {
        if (buildingUIPanel != null)
        {
            isPanelOpen = !isPanelOpen; // переключаем состояние панели

            buildingUIPanel.SetActive(isPanelOpen); // открываем или закрываем панель
        }
    }
}
