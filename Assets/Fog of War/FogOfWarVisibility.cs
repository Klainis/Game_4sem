using UnityEngine;

public class FogOfWarVisibility : MonoBehaviour
{
    public RenderTexture visibilityTexture; // �������� ������ �����
    public Camera fogCamera; // ������, ���������� �����
    public LayerMask fogLayer; // ���� ������

    private Texture2D readableTexture;
    private bool isVisible = true;

    void Start()
    {
        // ������� �������� ��� ������
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
        // Проверяем, что fogCamera назначена
        if (fogCamera == null)
        {
            isVisible = true;
            return;
        }

        // ������������ ������� ���������� � UV ��������
        Vector3 viewPos = fogCamera.WorldToViewportPoint(transform.position);

        // ���� ������ ��� ������ ������
        if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
        {
            isVisible = false;
            return;
        }

        // ������ ������� �� ��������
        RenderTexture.active = visibilityTexture;
        readableTexture.ReadPixels(new Rect(
            viewPos.x * visibilityTexture.width,
            viewPos.y * visibilityTexture.height,
            1, 1), 0, 0);
        readableTexture.Apply();
        RenderTexture.active = null;

        Color pixel = readableTexture.GetPixel(0, 0);
        isVisible = pixel.grayscale > 0.1f; // ��������� ��������
    }

    void UpdateVisibility()
    {
        // ����� ���������� �������/����� �������
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = isVisible;
        }

        // ������������ ��� Canvas ���������
        // GetComponent<CanvasGroup>().alpha = isVisible ? 1 : 0;
    }
}