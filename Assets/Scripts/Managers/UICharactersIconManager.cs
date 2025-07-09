using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharactersIconManager : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Transform iconsContainer; // Родительский объект для иконок

    [Header("Префабы юнитов (для автоматического определения иконок)")]
    [SerializeField] private List<GameObject> unitPrefabs; // Префабы всех юнитов

    private Dictionary<string, GameObject> unitIconsDictionary = new Dictionary<string, GameObject>();
    private List<GameObject> activeIcons = new List<GameObject>();

    private void Awake()
    {
        // Заполняем словарь иконок на основе префабов юнитов
        foreach (var unitPrefab in unitPrefabs)
        {
            var iconComponent = unitPrefab.GetComponent<UnitIcon>();
            if (iconComponent != null && iconComponent.iconPrefab != null)
            {
                string unitName = unitPrefab.name;
                if (!unitIconsDictionary.ContainsKey(unitName))
                {
                    unitIconsDictionary.Add(unitName, iconComponent.iconPrefab);
                }
            }
        }
    }

    // Обновляет иконки при изменении выделения
    public void UpdateSelectionIcons(List<GameObject> selectedUnits)
    {
        ClearIcons();

        foreach (var unit in selectedUnits)
        {
            string unitName = GetUnitName(unit);
            if (unitIconsDictionary.TryGetValue(unitName, out GameObject iconPrefab))
            {
                AddIcon(iconPrefab);
            }
        }
    }

    private string GetUnitName(GameObject unit)
    {
        // Получаем оригинальное имя префаба (без клонирования)
        if (unit.name.Contains("(Clone)"))
        {
            return unit.name.Replace("(Clone)", "").Trim();
        }
        return unit.name;
    }

    private void AddIcon(GameObject iconPrefab)
    {
        if (iconPrefab == null || iconsContainer == null) return;

        GameObject newIcon = Instantiate(iconPrefab, iconsContainer);
        activeIcons.Add(newIcon);
    }

    private void ClearIcons()
    {
        foreach (var icon in activeIcons)
        {
            Destroy(icon);
        }
        activeIcons.Clear();
    }
}
