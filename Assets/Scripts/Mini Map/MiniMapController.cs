using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private RawImage minimapRawImage;
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private Transform playerUnitsParent;
    [SerializeField] private Transform enemyUnitsParent;
    [SerializeField] private Transform enemyBuildingParent;
    [SerializeField] private Transform unitBuildingParent;

    [Header("Background")]
    [SerializeField] private Image backgroundImage; // Фоновое изображение
    [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f); // Цвет фона по умолчанию

    [Header("Visual Settings")]
    [SerializeField] private Color playerUnitColor = Color.green;
    [SerializeField] private Color enemyUnitColor = Color.red;
    [SerializeField] private Color enemyBuildingColor = Color.red;
    [SerializeField] private Color unitBuildingColor = Color.yellow;
    [SerializeField] private Vector2 unitIconSize = new Vector2(4, 4);
    [SerializeField] private Vector2 unitBuildingSize = new Vector2(8, 8);
    [SerializeField] private Vector2 enemyBuildingSize = new Vector2(8, 8);

    private Texture2D playerUnitTexture;
    private Texture2D enemyUnitTexture;
    private Texture2D enemyBuildingTexture;
    private Texture2D unitBuildingTexture;

    private void Awake()
    {
        CreateTextures();
        SetupBackground();
    }

    private void SetupBackground()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
            // Убедимся, что фон находится позади RawImage
            backgroundImage.transform.SetSiblingIndex(0);
        }
    }

    private Rect GetMinimapScreenRect()
    {
        RectTransform rt = minimapRawImage.rectTransform;
        Vector2 min = rt.TransformPoint(rt.rect.min);
        Vector2 max = rt.TransformPoint(rt.rect.max);
        return new Rect(min.x, Screen.height - max.y, max.x - min.x, max.y - min.y);
    }

    private void CreateTextures()
    {
        playerUnitTexture = CreateColoredTexture(playerUnitColor);
        enemyUnitTexture = CreateColoredTexture(enemyUnitColor);
        enemyBuildingTexture = CreateColoredTexture(enemyBuildingColor);
        unitBuildingTexture = CreateColoredTexture(unitBuildingColor);
    }

    private Texture2D CreateColoredTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    private void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;

        DrawUnits(playerUnitsParent, playerUnitTexture);
        DrawUnits(enemyUnitsParent, enemyUnitTexture);
        DrawBuildings(unitBuildingSize, unitBuildingParent, unitBuildingTexture);
        DrawBuildings(enemyBuildingSize, enemyBuildingParent, enemyBuildingTexture);
    }

    private void DrawUnits(Transform parent, Texture2D texture)
    {
        if (parent == null) return;
        
        Rect minimapRect = GetMinimapScreenRect();
        float minimapWidth = minimapRect.width;
        float minimapHeight = minimapRect.height;

        foreach (Transform unit in parent)
        {
            if (unit == null) continue;
            
            Vector3 worldPos = unit.position;
            Vector2 viewportPos = minimapCamera.WorldToViewportPoint(worldPos);

            // Проверяем, виден ли объект на мини-карте
            if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
                continue;

            // Рисуем в экранных координатах относительно мини-карты
            Rect iconRect = new Rect(
                minimapRect.x + viewportPos.y * minimapWidth - unitIconSize.x / 2,  // X → Y
                minimapRect.y + (1 - viewportPos.x) * minimapHeight - unitIconSize.y / 2,  // Y → 1-X
                unitIconSize.x,
                unitIconSize.y
            );

            GUI.DrawTexture(iconRect, texture);
        }
    }

    private void DrawBuildings(Vector2 buildingSize, Transform buildingsParent, Texture2D texture)
    {
        if (buildingsParent == null) return;
        
        Rect minimapRect = GetMinimapScreenRect();
        float minimapWidth = minimapRect.width;
        float minimapHeight = minimapRect.height;

        foreach (Transform build in buildingsParent)
        {
            if (build == null) continue;
            
            Vector3 worldPos = build.position;
            Vector2 viewportPos = minimapCamera.WorldToViewportPoint(worldPos);

            // Проверяем, виден ли объект на мини-карте
            if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
                continue;

            // Рисуем в экранных координатах относительно мини-карты
            Rect iconRect = new Rect(
                minimapRect.x + viewportPos.y * minimapWidth - buildingSize.x / 2,
                minimapRect.y + (1 - viewportPos.x) * minimapHeight - buildingSize.y / 2,
                buildingSize.x,
                buildingSize.y
            );

            GUI.DrawTexture(iconRect, texture);
        }
    }
}