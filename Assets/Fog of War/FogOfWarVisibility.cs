using UnityEngine;

public class FogOfWarVisibility : MonoBehaviour
{
    public RenderTexture visibilityTexture; // Текстура тумана войны
    public Camera fogCamera; // Камера, рендерящая туман
    public LayerMask fogLayer; // Слой тумана

    private Texture2D readableTexture;
    private bool isVisible = true;

    void Start()
    {
        // Создаем текстуру для чтения
        readableTexture = new Texture2D(
            visibilityTexture.width,
            visibilityTexture.height,
            TextureFormat.RGBA32,
            false);
    }

    void Update()
    {
        CheckVisibility();
        UpdateVisibility();
    }

    void CheckVisibility()
    {
        // Конвертируем мировые координаты в UV текстуры
        Vector3 viewPos = fogCamera.WorldToViewportPoint(transform.position);

        // Если объект вне камеры тумана
        if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
        {
            isVisible = false;
            return;
        }

        // Читаем пиксель из текстуры
        RenderTexture.active = visibilityTexture;
        readableTexture.ReadPixels(new Rect(
            viewPos.x * visibilityTexture.width,
            viewPos.y * visibilityTexture.height,
            1, 1), 0, 0);
        readableTexture.Apply();
        RenderTexture.active = null;

        Color pixel = readableTexture.GetPixel(0, 0);
        isVisible = pixel.grayscale > 0.1f; // Пороговое значение
    }

    void UpdateVisibility()
    {
        // Здесь реализуйте скрытие/показ объекта
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = isVisible;
        }

        // Альтернатива для Canvas элементов
        // GetComponent<CanvasGroup>().alpha = isVisible ? 1 : 0;
    }
}