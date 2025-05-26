using UnityEngine;

/// <summary>
/// Отвечает только за визуальный «призрак», проверку валидности
/// и фактическое создание здания.  Ввод получает через
/// PlacementInputHandler.
/// </summary>
public class BuildingPlacementManager : MonoBehaviour
{
    /*–––– ссылки ––––*/
    [SerializeField] PlacementInputHandler input;        // drag & drop в инспекторе
    [SerializeField] Material ghostMaterial;
    [SerializeField] LayerMask groundLayer;

    /*–––– настройки ––––*/
    [Header("Grid")]
    [SerializeField] float gridSize = 1f;
    [SerializeField] bool  snapToGrid = true;

    [Header("Rotation")]
    [SerializeField] int rotationStep = 90;

    [Header("Blocking tags")]
    [SerializeField] string[] blockingTags = { "Building" };

    /*–––– runtime ––––*/
    GameObject  ghostInstance;
    Renderer[]  ghostRenderers;
    GameObject  currentPrefab;
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
        SetGhostColor(IsPlacementValid(ghostInstance.transform.position));
    }

    /*–––– public API ––––*/
    public void BeginPlacement(GameObject prefab, int cost)
    {
        CancelPlacement();                     // сбрасываем предыдущую попытку

        currentPrefab = prefab;
        currentCost   = cost;

        ghostInstance = Instantiate(currentPrefab);
        ghostInstance.transform.rotation = currentPrefab.transform.rotation;

        var allRV = ghostInstance.GetComponentsInChildren<RangeVisualizer>(true);
        foreach (var rv in allRV)
            rv.Show();

        SetGhostAppearance(ghostInstance);
        ghostRenderers = ghostInstance.GetComponentsInChildren<Renderer>();

        isPlacing = true;
    }

    public void CancelPlacement()
    {
        if (ghostInstance) Destroy(ghostInstance);
        isPlacing     = false;
        currentPrefab = null;
    }

    /*–––– event-handlers ––––*/
    void OnRotate()
    {
        if (isPlacing && ghostInstance)
            ghostInstance.transform.Rotate(Vector3.up, rotationStep, Space.World);
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

            // Активируем интерактивность для производственных зданий
            go.GetComponent<ProductionBuilding>()?.EnableInteraction();

            // Вызываем OnPlaced для пушек и других зданий
            go.GetComponent<Cannon>()?.OnPlaced();

            // Начинаем размещение следующего здания
            BeginPlacement(currentPrefab, currentCost);
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
            ghostInstance.transform.position = pos;
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
        if (ghostRenderers == null) return;
        var c = (isValid ? Color.green : Color.red);  c.a = .5f;
        foreach (var r in ghostRenderers) r.material.color = c;
    }

    Vector3 GetGhostColliderSize()
    {
        if (ghostInstance.TryGetComponent(out BoxCollider col))
            return Vector3.Scale(col.size, ghostInstance.transform.localScale);

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
        return true;
    }
}
