using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UnitButton : MonoBehaviour
{
    void Awake()
    {
        // Убедимся, что кнопка растягивается на всю ячейку
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
    }
} 