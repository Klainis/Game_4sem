using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementManager : MonoBehaviour
{
    /*–––– настройки ––––*/
    [Header("Коллизии по тегам")]
    [SerializeField] string[] blockingTags = { "Building" };

    [Header("Ghost & проверка")]
    [SerializeField] Material ghostMaterial;
    [SerializeField] LayerMask groundLayer;

    [Header("Сетка")]
    [SerializeField] float gridSize = 1f;            // шаг сетки
    [SerializeField] bool snapToGrid = true;         // можно выключить в инспекторе

    [Header("Поворот")]
    [SerializeField] KeyCode rotateKey = KeyCode.R;  // клавиша поворота
    [SerializeField] int rotationStep = 90;          // угол одного шага

    /*–––– внутренние данные ––––*/
    GameObject ghostInstance;
    Renderer[] ghostRenderers;
    GameObject currentPrefab;
    bool isPlacing;

    /*–––– публичный API ––––*/
    public void BeginPlacement(GameObject prefab)
    {
        CancelPlacement();
        currentPrefab = prefab;
        ghostInstance = Instantiate(currentPrefab);
        SetGhostAppearance(ghostInstance);
        ghostRenderers = ghostInstance.GetComponentsInChildren<Renderer>();
        isPlacing = true;
    }

    public void CancelPlacement()
    {
        if (ghostInstance) Destroy(ghostInstance);
        isPlacing = false;
        currentPrefab = null;
    }

    void Update()
    {
        if (!isPlacing) return;

        /*–– 1. Поворот ––*/
        if (Input.GetKeyDown(rotateKey))
        {
            ghostInstance.transform.Rotate(Vector3.up, rotationStep, Space.World);
        }


        /*–– 2. Обновление позиции ––*/
        UpdateGhostPosition();

        bool valid = IsPlacementValid(ghostInstance.transform.position);
        SetGhostColor(valid);

        /*–– 3. Установка здания, если курсор не над UI ––*/
        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButtonDown(0) && valid && !pointerOverUI)
        {
            Instantiate(currentPrefab,
                        ghostInstance.transform.position,
                        ghostInstance.transform.rotation);      // ← сохранит угол
            // остаёмся в режиме размещения
        }

        /*–– 4. Выход из режима ––*/
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    /*–––– вспомогательные методы ––––*/

    private void UpdateGhostPosition()
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

    private void SetGhostAppearance(GameObject ghost)
    {
        Renderer[] renderers = ghost.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material ghostMat = new Material(ghostMaterial != null ? ghostMaterial : renderer.material);
            Color color = ghostMat.color;
            color.a = 0.5f;
            ghostMat.color = color;
            renderer.material = ghostMat;
        }
    }

    private void SetGhostColor(bool isValid)
    {
        if (ghostRenderers == null) return;

        Color color = isValid ? Color.green : Color.red;
        color.a = 0.5f;

        foreach (Renderer renderer in ghostRenderers)
        {
            renderer.material.color = color;
        }
    }

    private Vector3 GetGhostColliderSize()
    {
        BoxCollider collider = ghostInstance.GetComponent<BoxCollider>();
        if (collider != null)
        {
            Vector3 scaledSize = Vector3.Scale(collider.size, ghostInstance.transform.localScale);
            return scaledSize;
        }

        return new Vector3(1.1f, 1.5f, 1.1f); // fallback
    }

    private bool IsPlacementValid(Vector3 position)
    {
        Vector3 size   = GetGhostColliderSize();
        Vector3 center = position + Vector3.up * (size.y / 2);
        Quaternion rot = ghostInstance.transform.rotation;

        // Проверяем всё, без LayerMask
        Collider[] hits = Physics.OverlapBox(center, size / 2f, rot);

        foreach (var hit in hits)
        {
            if (hit.gameObject == ghostInstance) continue;          // пропускаем призрак

            foreach (var tag in blockingTags)                       // если тег блокирует — нельзя
                if (hit.CompareTag(tag)) return false;
        }
        return true;                                                // свободно
    }


    private void OnDrawGizmosSelected()
    {
        if (ghostInstance != null)
        {
            Gizmos.color = Color.red;
            Vector3 size = GetGhostColliderSize();
            Vector3 center = ghostInstance.transform.position + Vector3.up * (size.y / 2);
            Quaternion rotation = ghostInstance.transform.rotation;

            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
