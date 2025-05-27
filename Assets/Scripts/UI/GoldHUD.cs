using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Слушает ResourceManager и обновляет надпись при изменении золота.
/// Подписка происходит в Start(), чтобы гарантированно был установлен Instance.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class GoldHUD : MonoBehaviour
{
    [SerializeField] private Image goldIcon; // Иконка золота
    private TMP_Text label;

    void Awake()
    {
        // Кэшируем ссылку на текст
        label = GetComponent<TMP_Text>();
        
        // Настраиваем отступ текста, чтобы он не перекрывался с иконкой
        if (goldIcon != null)
        {
            label.margin = new Vector4(goldIcon.rectTransform.rect.width + 5, 0, 0, 0);
        }
    }

    void Start()
    {
        // Подписываемся на событие изменения золота
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnGoldChanged += UpdateLabel;
            // И сразу обновляем текущее значение
            UpdateLabel(ResourceManager.Instance.Gold);
        }
        else
        {
            Debug.LogError("GoldHUD: нет ResourceManager.Instance при Start()");
        }
    }

    void OnDestroy()
    {
        // Отписываемся, чтобы не было утечек
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnGoldChanged -= UpdateLabel;
    }

    void UpdateLabel(int newGold)
    {
        label.text = newGold.ToString();
    }
}
