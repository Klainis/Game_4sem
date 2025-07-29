using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UnitSelectedPanel : MonoBehaviour
{
    [SerializeField] private Transform iconContainer;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Sprite[] unitIcons;

    [SerializeField] private Color GreenUIHP;
    [SerializeField] private Color YellowUIHP;
    [SerializeField] private Color RedUIHP;

    private List<IconUI> currentIcons = new List<IconUI>();

    void Update()
    {
        foreach (var icon in currentIcons)
        {
            if (icon.unit != null)
            {
                icon.hpBar.value = icon.unit.unitHealth;
                float healthPercentage = Mathf.Clamp01(icon.hpBar.value / icon.unit.unitMaxHealth);
                UpdateColor(healthPercentage, icon.sliderFill);
            }
        }
    }

    public void UpdateUnitIcons(List<GameObject> selectedUnits)
    {
        ClearIcons();

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            var unit = selectedUnits[i];
            var unitComp = unit.GetComponent<Unit>();

            GameObject iconGO = Instantiate(iconPrefab, iconContainer);
            iconGO.GetComponent<Image>().sprite = unitIcons[unitComp.iconID];

            Slider hpBar = iconGO.transform.Find("HPBar").GetComponent<Slider>();
            hpBar.maxValue = unitComp.unitMaxHealth;
            hpBar.value = unitComp.unitHealth;

            Image hpFill = hpBar.fillRect.GetComponent<Image>();
            Debug.Log($"Изначальный цвет иконки:  { hpFill.color} ");

            currentIcons.Add(new IconUI
            {
                unit = unitComp,
                hpBar = hpBar,
                sliderFill = hpFill,

            });

            int index = i;
            //iconGO.GetComponent<Button>().onClick.AddListener(() =>
            //{
            //    UnitSelectionManager.Instance.DeselectAll();
            //    UnitSelectionManager.Instance.unitSelected.Add(unit);
            //    UnitSelectionManager.Instance.TriggerSelectionIndicator(unit, true);
            //});
        }
    }

    private void ClearIcons()
    {
        foreach (var icon in currentIcons)
            Destroy(icon.hpBar.transform.parent.gameObject); // удалить весь prefab
        currentIcons.Clear();
    }

    public void RemoveUnitIcon(Unit unit)
    {
        for (int i = 0; i < currentIcons.Count; i++)
        {
            if (currentIcons[i].unit == unit)
            {
                Destroy(currentIcons[i].hpBar.transform.parent.gameObject);
                currentIcons.RemoveAt(i);
                break;
            }
        }
    }

    private void UpdateColor(float healthPercentage, Image sliderFill)
    {
        if (healthPercentage >= 0.6f)
        {
            //Debug.Log(healthPercentage);
            sliderFill.color = GreenUIHP;
        }
        else if (healthPercentage >= 0.3f)
        {
            //Debug.Log(healthPercentage);
            sliderFill.color = YellowUIHP;
        }
        else
        {
            //Debug.Log(healthPercentage);
            sliderFill.color = RedUIHP;
        }
    }

    private class IconUI
    {
        public Unit unit;
        public Slider hpBar;
        public Image sliderFill;
    }
}
