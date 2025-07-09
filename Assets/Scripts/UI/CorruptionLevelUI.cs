using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CorruptionLevelUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider corruptionSlider;
    [SerializeField] private TextMeshProUGUI corruptionText;
    [SerializeField] private Image corruptionIcon;

    [Header("Settings")]
    [SerializeField] private Color lowCorruptionColor = Color.green;
    [SerializeField] private Color mediumCorruptionColor = Color.yellow;
    [SerializeField] private Color highCorruptionColor = Color.red;

    private void Start()
    {
        if (CorruptionGridManager.Instance != null)
        {
            CorruptionGridManager.Instance.OnCorruptionLevelChanged += UpdateCorruptionUI;
            UpdateCorruptionUI(CorruptionGridManager.Instance.GetCorruptionLevel());
        }
    }

    private void OnDestroy()
    {
        if (CorruptionGridManager.Instance != null)
        {
            CorruptionGridManager.Instance.OnCorruptionLevelChanged -= UpdateCorruptionUI;
        }
    }

    private void UpdateCorruptionUI(float corruptionLevel)
    {
        // Обновляем слайдер
        if (corruptionSlider != null)
        {
            corruptionSlider.value = corruptionLevel;
        }

        // Обновляем текст
        if (corruptionText != null)
        {
            corruptionText.text = $"Заражение: {corruptionLevel:P0}";
        }

        // Обновляем цвет иконки
        if (corruptionIcon != null)
        {
            if (corruptionLevel < 0.3f)
                corruptionIcon.color = lowCorruptionColor;
            else if (corruptionLevel < 0.7f)
                corruptionIcon.color = mediumCorruptionColor;
            else
                corruptionIcon.color = highCorruptionColor;
        }
    }
} 