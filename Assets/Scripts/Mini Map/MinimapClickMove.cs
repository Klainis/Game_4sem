using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MinimapClickMove : MonoBehaviour
{
    public Camera minimapCamera;  // Ортографическая камера миникарты
    public Transform player;      // Игрок
    public NavMeshAgent agent;    // NavMeshAgent игрока
    public LayerMask terrainLayer; // Слой ландшафта

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePos = Input.mousePosition;

            // Проверяем, попадает ли клик в область миникарты (если миникарта часть UI)
            if (IsPointerOverMinimap(mousePos))
            {
                Vector3 worldPos = ScreenToWorldOnTerrain(mousePos);
                if (worldPos != Vector3.zero)
                {
                    agent.SetDestination(worldPos);
                }
            }
        }
    }

    bool IsPointerOverMinimap(Vector3 screenPos)
    {
        Rect minimapRect = new Rect(10, 10, 150, 150);
        return minimapRect.Contains(screenPos);
    }

    Vector3 ScreenToWorldOnTerrain(Vector3 screenPos)
    {
        // Преобразуем позицию мыши к координатам миникарты
        Ray ray = minimapCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, terrainLayer))
        {
            return hit.point;
        }

        return Vector3.zero; // Ничего не найдено
    }
}
