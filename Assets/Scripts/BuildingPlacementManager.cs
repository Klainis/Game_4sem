using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementManager : MonoBehaviour
{
    [Header("Ghost & проверка")]
    [SerializeField] Material ghostMaterial;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask collisionLayers;

    GameObject ghostInstance;
    Renderer[] ghostRenderers;
    GameObject currentPrefab;     // выбранная постройка
    bool isPlacing;

    /*–– публичный API ––*/
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

        UpdateGhostPosition();

        bool valid = IsPlacementValid(ghostInstance.transform.position);
        SetGhostColor(valid);

        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButtonDown(0) && valid && !pointerOverUI)
        {
            Instantiate(currentPrefab,
                        ghostInstance.transform.position,
                        ghostInstance.transform.rotation);
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    private void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            ghostInstance.transform.position = hit.point;
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
        Vector3 size = GetGhostColliderSize();
        Vector3 center = position + Vector3.up * (size.y / 2);
        Quaternion rotation = ghostInstance.transform.rotation;

        Collider[] hits = Physics.OverlapBox(center, size / 2f, rotation, collisionLayers);

        foreach (var hit in hits)
        {
            if (ghostInstance != null && hit.gameObject == ghostInstance) continue;
            return false;
        }

        return true;
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
