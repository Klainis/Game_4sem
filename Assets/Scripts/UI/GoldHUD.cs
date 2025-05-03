using TMPro;
using UnityEngine;

/// <summary>
/// Слушает ResourceManager и обновляет надпись при изменении золота.
/// Подписка происходит в Start(), чтобы гарантированно был установлен Instance.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class GoldHUD : MonoBehaviour
{
    TMP_Text label;

    void Awake()
    {
        // Кэшируем ссылку на текст
        label = GetComponent<TMP_Text>();
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
        label.text = $"Gold: {newGold}";
    }
}
