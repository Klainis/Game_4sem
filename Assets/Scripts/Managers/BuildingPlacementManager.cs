using UnityEngine;

/// <summary>
/// Отвечает только за визуальный «призрак», проверку валидности
/// и фактическое создание здания.  Ввод получает через
/// PlacementInputHandler. Поддерживает отдельные ghost-префабы.
/// </summary>
public class BuildingPlacementManager : MonoBehaviour
{
    /*–––– ссылки ––––*/
    [SerializeField] PlacementInputHandler input;        // drag & drop в инспекторе
    [SerializeField] Material ghostMaterial;             // Fallback материал для старых зданий
    [SerializeField] LayerMask groundLayer;

    /*–––– настройки ––––*/
    [Header("Grid")]
    [SerializeField] float gridSize = 1f;
    [SerializeField] bool  snapToGrid = true;

    [Header("Prefab`s parent'")]
    [SerializeField] Transform unitParent;
    [SerializeField] Transform buildingParent;

    [Header("Rotation")]
    [SerializeField] int rotationStep = 90;

    [Header("Blocking tags")]
    [SerializeField] string[] blockingTags = { "Building" };

    /*–––– runtime ––––*/
    GameObject  ghostInstance;
    BuildingGhost buildingGhost;         // Компонент ghost-объекта
    Renderer[]  ghostRenderers;         // Fallback для старых зданий
    GameObject  currentPrefab;          // Оригинальный префаб здания
    GameObject  currentGhostPrefab;     // Ghost-префаб (если есть)
    int         currentCost;
    bool        isPlacing;

    // Публичное свойство для проверки текущего призрака
    public GameObject CurrentGhost => ghostInstance;

    /*–––– life-cycle ––––*/
    void OnEnable()
    {
        if (!input) input = FindObjectOfType<PlacementInputHandler>();

        input.RotateRequested += OnRotate;
        input.CancelRequested += CancelPlacement;
        input.PlaceRequested  += TryPlaceBuilding;
    }

    void OnDisable()
    {
        input.RotateRequested -= OnRotate;
        input.CancelRequested -= CancelPlacement;
        input.PlaceRequested  -= TryPlaceBuilding;
    }

    void Update()
    {
        if (!isPlacing) return;

        UpdateGhostPosition();
        bool isValid = IsPlacementValid(ghostInstance.transform.position);
        SetGhostColor(isValid);
    }

    /*–––– public API ––––*/
    public void BeginPlacement(GameObject prefab, int cost)
    {
        BeginPlacement(prefab, null, cost);
    }

    public void BeginPlacement(GameObject prefab, GameObject ghostPrefab, int cost)
    {
        CancelPlacement();                     // сбрасываем предыдущую попытку

        currentPrefab = prefab;
        currentGhostPrefab = ghostPrefab;
        currentCost = cost;

        // Используем ghost-префаб если он есть, иначе создаем ghost из обычного префаба
        GameObject prefabToInstantiate = currentGhostPrefab != null ? currentGhostPrefab : currentPrefab;
        ghostInstance = Instantiate(prefabToInstantiate, prefabToInstantiate.transform.position, prefabToInstantiate.transform.rotation);

        // Проверяем, есть ли компонент BuildingGhost
        buildingGhost = ghostInstance.GetComponent<BuildingGhost>();

        if (buildingGhost != null)
        {
            // Используем новую систему ghost-объектов
            Debug.Log($"[BuildingPlacementManager] Using new ghost system for {prefab.name}");
        }
        else
        {
            // Fallback: используем старую систему
            Debug.Log($"[BuildingPlacementManager] Using legacy ghost system for {prefab.name}");
            CreateLegacyGhost();
        }

        isPlacing = true;
    }

    /// <summary>
    /// Создает ghost в старом стиле для совместимости
    /// </summary>
    private void CreateLegacyGhost()
    {
        var allRV = ghostInstance.GetComponentsInChildren<RangeVisualizer>(true);
        foreach (var rv in allRV)
            rv.Show();

        SetGhostAppearance(ghostInstance);
        ghostRenderers = ghostInstance.GetComponentsInChildren<Renderer>();
    }

    public void CancelPlacement()
    {
        if (ghostInstance) Destroy(ghostInstance);
        isPlacing = false;
        currentPrefab = null;
        currentGhostPrefab = null;
        buildingGhost = null;
        ghostRenderers = null;
    }

    /*–––– event-handlers ––––*/
    void OnRotate()
    {
        if (!isPlacing || !ghostInstance) return;

        if (buildingGhost != null)
        {
            buildingGhost.Rotate(rotationStep);
        }
        else
        {
            ghostInstance.transform.Rotate(Vector3.up, rotationStep, Space.World);
        }
    }

    void TryPlaceBuilding(Vector3 clickPoint)
    {
        if (!isPlacing) return;

        if (!IsPlacementValid(ghostInstance.transform.position)) return;

        if (ResourceManager.Instance.SpendGold(currentCost))
        {
            // Создаем здание
            var go = Instantiate(currentPrefab,
                        ghostInstance.transform.position,
                        ghostInstance.transform.rotation);
            //Добавление в родителя для отображения на карте
            if (go.CompareTag("Friendly"))
            {
                go.transform.SetParent(unitParent);
            }
            else if (go.CompareTag("Building"))
            {
                go.transform.SetParent(buildingParent);
            }

            // Активируем интерактивность для производственных зданий
            go.GetComponent<ProductionBuilding>()?.EnableInteraction();

            // Вызываем OnPlaced для пушек и других зданий
            go.GetComponent<Cannon>()?.OnPlaced();
            go.GetComponent<TempleOfPurity>()?.OnPlaced();

            // Начинаем размещение следующего здания
            BeginPlacement(currentPrefab, currentGhostPrefab, currentCost);
        }
        else
        {
            Debug.Log("Недостаточно золота для постройки!");
        }
    }

    /*–––– helpers ––––*/
    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
        {
            Vector3 pos = hit.point;

            if (snapToGrid)
            {
                pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
                pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
            }

            if (buildingGhost != null)
            {
                buildingGhost.SetPosition(pos);
            }
            else
            {
                ghostInstance.transform.position = pos;
            }
        }
    }

    void SetGhostAppearance(GameObject ghost)
    {
        foreach (var rend in ghost.GetComponentsInChildren<Renderer>())
        {
            var mat = new Material(ghostMaterial ? ghostMaterial : rend.material);
            var c   = mat.color;  c.a = .5f;  mat.color = c;
            rend.material = mat;
        }
    }

    void SetGhostColor(bool isValid)
    {
        if (buildingGhost != null)
        {
            buildingGhost.SetValidationColor(isValid);
        }
        else if (ghostRenderers != null)
        {
            // Fallback для старой системы
            var c = (isValid ? Color.green : Color.red);  
            c.a = .5f;
            foreach (var r in ghostRenderers) 
            {
                if (r != null && r.material != null)
                    r.material.color = c;
            }
        }
    }

    Vector3 GetGhostColliderSize()
    {
        if (buildingGhost != null)
        {
            return buildingGhost.GetColliderSize();
        }
        else if (ghostInstance.TryGetComponent(out BoxCollider col))
        {
            return Vector3.Scale(col.size, ghostInstance.transform.localScale);
        }

        return new Vector3(1.1f, 1.5f, 1.1f); // fallback
    }

    bool IsPlacementValid(Vector3 pos)
    {
        var size   = GetGhostColliderSize();
        var center = pos + Vector3.up * (size.y / 2);
        var hits   = Physics.OverlapBox(center, size / 2, ghostInstance.transform.rotation);

        foreach (var h in hits)
        {
            if (h.gameObject == ghostInstance) continue;
            foreach (var tag in blockingTags)
                if (h.CompareTag(tag)) return false;
        }

        // Проверка на скверну (запрет строительства на заражённых клетках)
        if (CorruptionGridManager.Instance != null)
        {
            if (!CorruptionGridManager.Instance.CanBuildAt(pos))
                return false;
        }

        return true;
    }
}
