using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuildingOption
{
    public string name;
    public GameObject prefab;
    public int cost;
    public Sprite icon; // Иконка здания
}

public class BuildingUI : MonoBehaviour
{
    [Header("References")]
    public BuildingPlacementManager placementManager;
    public BuildingOption[] buildingOptions;
    public GameObject floatingTextPrefab;

    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage; // Фоновое изображение
    [SerializeField] private GridLayoutGroup buttonLayout; // Компонент для расположения кнопок
    [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Цвет фона

    private void Awake()
    {
        SetupPanel();
    }

    private void SetupPanel()
    {
        // Настраиваем фон
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
            // Убедимся, что фон находится позади всех элементов
            backgroundImage.transform.SetSiblingIndex(0);
        }

        // Настраиваем layout для кнопок, если он есть
        if (buttonLayout != null)
        {
            buttonLayout.cellSize = new Vector2(160, 45); // Размер кнопок
            buttonLayout.spacing = new Vector2(10, 10); // Расстояние между кнопками
            buttonLayout.padding = new RectOffset(10, 10, 10, 10); // Отступы от краёв
            buttonLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            buttonLayout.constraintCount = 1; // Одна кнопка в строке
        }

        // Скрываем панель при старте
        gameObject.SetActive(false);
    }

    public void SelectBuilding(int index)
    {
        if (placementManager != null && index >= 0 && index < buildingOptions.Length)
        {
            placementManager.BeginPlacement(buildingOptions[index].prefab, buildingOptions[index].cost);
        }
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
