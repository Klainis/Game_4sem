using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionGridManager : MonoBehaviour
{
    public static CorruptionGridManager Instance { get; private set; }

    public enum CellState
    {
        Clean,
        Corrupted
    }

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 gridOrigin;
    [SerializeField] private GameObject corruptionOverlayPrefab;
    [SerializeField] private Material corruptionMaterial;
    [SerializeField] private Color corruptionColor;
    [SerializeField] private float corruptionSpreadInterval;
    [SerializeField] private int corruptionSpreadRadius;

    private CellState[,] corruptionMap;
    private GameObject[,] corruptionOverlays;
    private int corruptedCells;
    public int TotalCells => gridWidth * gridHeight;
    public float corruptionLevel => TotalCells > 0 ? (float)corruptedCells / TotalCells : 0f;

    public System.Action<Vector2Int> OnCellCorrupted;
    public System.Action<Vector2Int> OnCellCleaned;
    public System.Action<float> OnCorruptionLevelChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        corruptionMap = new CellState[gridWidth, gridHeight];
        corruptionOverlays = new GameObject[gridWidth, gridHeight];
        corruptedCells = 0;
        // Можно инициализировать стартовые заражённые зоны здесь
        CorruptArea(new Vector2Int(gridWidth / 2, gridHeight / 2), 4);
        StartCoroutine(CorruptionSpreadRoutine());
    }

    /// <summary>
    /// Заражает область вокруг центральной точки
    /// </summary>
    public void CorruptArea(Vector2Int center, int radius)
    {
        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int z = center.y - radius; z <= center.y + radius; z++)
            {
                if (IsValidGridPosition(x, z))
                {
                    float distance = Vector2Int.Distance(center, new Vector2Int(x, z));
                    if (distance <= radius)
                    {
                        CorruptCell(x, z);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Очищает область вокруг центральной точки
    /// </summary>
    public void CleanArea(Vector2Int center, int radius)
    {
        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int z = center.y - radius; z <= center.y + radius; z++)
            {
                if (IsValidGridPosition(x, z))
                {
                    float distance = Vector2Int.Distance(center, new Vector2Int(x, z));
                    if (distance <= radius)
                    {
                        CleanCell(x, z);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Заражает одну клетку
    /// </summary>
    private void CorruptCell(int x, int z)
    {
        if (corruptionMap[x, z] == CellState.Corrupted)
            return;

        corruptionMap[x, z] = CellState.Corrupted;
        corruptedCells++;
        UpdateOverlay(x, z, true);

        OnCellCorrupted?.Invoke(new Vector2Int(x, z));
        OnCorruptionLevelChanged?.Invoke(corruptionLevel);
    }

    /// <summary>
    /// Очищает одну клетку
    /// </summary>
    private void CleanCell(int x, int z)
    {
        if (corruptionMap[x, z] == CellState.Clean)
            return;

        corruptionMap[x, z] = CellState.Clean;
        corruptedCells = Mathf.Max(0, corruptedCells - 1);
        UpdateOverlay(x, z, false);

        OnCellCleaned?.Invoke(new Vector2Int(x, z));
        OnCorruptionLevelChanged?.Invoke(corruptionLevel);
    }

    /// <summary>
    /// Проверяет, можно ли строить на клетке
    /// </summary>
    public bool CanBuildAt(Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGrid(worldPosition);
        if (!IsValidGridPosition(gridPos.x, gridPos.y))
            return true; // вне сетки — считаем безопасным

        return corruptionMap[gridPos.x, gridPos.y] != CellState.Corrupted;
    }

    /// <summary>
    /// Проверяет, заражена ли клетка
    /// </summary>
    public bool IsCorrupted(Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGrid(worldPosition);
        if (!IsValidGridPosition(gridPos.x, gridPos.y))
            return false;

        return corruptionMap[gridPos.x, gridPos.y] == CellState.Corrupted;
    }

    /// <summary>
    /// Вспомогательные методы
    /// </summary>
    private bool IsValidGridPosition(int x, int z)
    {
        return x >= 0 && x < gridWidth && z >= 0 && z < gridHeight;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - gridOrigin.x) / cellSize);
        int z = Mathf.FloorToInt((worldPos.z - gridOrigin.z) / cellSize);
        return new Vector2Int(x, z);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        float x = gridOrigin.x + gridPos.x * cellSize + cellSize / 2f;
        float z = gridOrigin.z + gridPos.y * cellSize + cellSize / 2f;
        return new Vector3(x, gridOrigin.y, z);
    }

    /// <summary>
    /// Визуализация заражения
    /// </summary>
    private void UpdateOverlay(int x, int z, bool show)
    {
        if (corruptionOverlayPrefab == null)
            return;

        if (show)
        {
            if (corruptionOverlays[x, z] == null)
            {
                Vector3 pos = GridToWorld(new Vector2Int(x, z));
                var overlay = Instantiate(corruptionOverlayPrefab, pos, corruptionOverlayPrefab.transform.rotation, transform);
                overlay.transform.localScale = Vector3.one * cellSize;
                var renderer = overlay.GetComponent<Renderer>();
                if (renderer && corruptionMaterial)
                {
                    renderer.material = corruptionMaterial;
                    renderer.material.color = corruptionColor;
                }
                corruptionOverlays[x, z] = overlay;
            }
        }
        else
        {
            if (corruptionOverlays[x, z] != null)
            {
                Destroy(corruptionOverlays[x, z]);
                corruptionOverlays[x, z] = null;
            }
        }
    }

    /// <summary>
    /// Автоматическое распространение скверны
    /// </summary>
    private IEnumerator CorruptionSpreadRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(corruptionSpreadInterval);

            List<Vector2Int> newCorrupted = new List<Vector2Int>();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    if (corruptionMap[x, z] == CellState.Corrupted)
                    {
                        // Проверка на наличие Храма Чистоты поблизости (опционально)
                        if (TempleOfPurityNearby(new Vector2Int(x, z)))
                            continue;

                        // Заражаем соседние клетки
                        for (int dx = -corruptionSpreadRadius; dx <= corruptionSpreadRadius; dx++)
                        {
                            for (int dz = -corruptionSpreadRadius; dz <= corruptionSpreadRadius; dz++)
                            {
                                int nx = x + dx;
                                int nz = z + dz;
                                if (IsValidGridPosition(nx, nz) && corruptionMap[nx, nz] == CellState.Clean)
                                {
                                    float dist = Vector2Int.Distance(new Vector2Int(x, z), new Vector2Int(nx, nz));
                                    if (dist <= corruptionSpreadRadius)
                                        newCorrupted.Add(new Vector2Int(nx, nz));
                                }
                            }
                        }
                    }
                }
            }

            foreach (var pos in newCorrupted)
            {
                CorruptCell(pos.x, pos.y);
            }
        }
    }

    // Проверка на наличие Храма Чистоты поблизости
    private bool TempleOfPurityNearby(Vector2Int cell)
    {
        Vector3 worldPos = GridToWorld(cell);
        float searchRadius = 5f; // Радиус поиска храмов
        
        Collider[] colliders = Physics.OverlapSphere(worldPos, searchRadius);
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<TempleOfPurity>() != null)
            {
                return true;
            }
        }
        return false;
    }

    // Для UI: получить уровень заражения (0..1)
    public float GetCorruptionLevel() => corruptionLevel;

    // Для дебаффов: проверить заражённость по worldPos
    public bool IsCorruptedCell(Vector3 worldPos) => IsCorrupted(worldPos);

    // Для запрета строительства: использовать CanBuildAt

    // Для TempleOfPurity: вызывать CleanArea(...)

    // Для врагов: вызывать CorruptArea(...)

    // Для визуализации: corruptionOverlayPrefab должен быть простым quad/plane с прозрачным фиолетовым материалом
}