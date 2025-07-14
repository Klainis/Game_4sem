using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject content;

    [Header("Color Settings")]
    [SerializeField] private Color startColor = Color.red;
    [SerializeField] private Color endColor = Color.green;

    private void Awake()
    {
        if(progressBar == null) progressBar = GetComponentInChildren<Slider>();
        if(progressText == null) progressText = GetComponentInChildren<TextMeshProUGUI>();
        if(fillImage == null && progressBar != null) fillImage = progressBar.fillRect.GetComponent<Image>();
        if(content == null) content = progressBar?.gameObject;
        
        // Убеждаемся, что Canvas настроен правильно для World Space
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            // Сбрасываем rotation Canvas к нулю, чтобы он был горизонтальным
            transform.rotation = Quaternion.identity;
            
            // Устанавливаем правильный масштаб
            transform.localScale = Vector3.one * 0.01f; // Маленький масштаб для World Space
        }
        
        Hide();
    }

    public void UpdateProgress(float progress, string text = "")
    {
        if (content != null && !content.activeSelf)
        {
            Show();
        }

        progress = Mathf.Clamp01(progress);

        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(startColor, endColor, progress);
        }

        if (progressText != null)
        {
            progressText.text = text;
        }
    }

    public void Show()
    {
        if(content != null) content.SetActive(true);
        // Принудительно обновляем позицию и поворот при показе
        ForceCorrectOrientation();
    }

    public void Hide()
    {
        if(content != null) content.SetActive(false);
    }

    /// <summary>
    /// Принудительно устанавливает правильную ориентацию Canvas
    /// </summary>
    public void ForceCorrectOrientation()
    {
        transform.rotation = Quaternion.identity;
        
        // Поворачиваем Canvas лицом к камере, но сохраняем горизонтальность
        if (Camera.main != null)
        {
            Vector3 lookDirection = Camera.main.transform.forward;
            lookDirection.y = 0; // Убираем вертикальную составляющую
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }
} 