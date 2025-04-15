using UnityEngine;

public class BuildingPlacementManager : MonoBehaviour
{
    [Header("Настройки постройки")]
    // Префаб здания, который будет ставиться (без ghost-эффектов)
    public GameObject buildingPrefab;

    // Материал для отображения ghost-версии (можно задать в инспекторе с прозрачностью)
    public Material ghostMaterial;

    // Слой, на который производим размещение (например, "Ground")
    public LayerMask groundLayer;

    // Временный объект для превью
    private GameObject ghostInstance;

    void Update()
    {
        // Активация режима построек по нажатию клавиши "B"
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Если ghost уже не создан и префаб назначен
            if (ghostInstance == null && buildingPrefab != null)
            {
                ghostInstance = Instantiate(buildingPrefab);
                SetGhostAppearance(ghostInstance);
            }
        }

        // Если ghost существует, обновляем позицию и обрабатываем ввод
        if (ghostInstance != null)
        {
            UpdateGhostPosition();

            // Если нажата левая кнопка мыши — пытаемся разместить постройку
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPlacementValid(ghostInstance.transform.position))
                {
                    // Создаем окончательную постройку
                    Instantiate(buildingPrefab, ghostInstance.transform.position, ghostInstance.transform.rotation);
                    Destroy(ghostInstance);
                }
            }
            // Правый клик для отмены размещения
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(ghostInstance);
            }
        }
    }

    // Метод обновления позиции ghost-объекта на основе положения курсора
    private void UpdateGhostPosition()
    {
        // Используем камеру из RTSCameraController (если она главная) или Camera.main
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            ghostInstance.transform.position = hit.point;
        }
    }

    // Настройка визуального эффекта ghost (прозрачный вид)
    private void SetGhostAppearance(GameObject ghost)
    {
        // Перебираем все рендереры в дочерних объектах
        Renderer[] renderers = ghost.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            // Клонируем материал, чтобы не изменить оригинал префаба
            Material ghostMat = new Material(ghostMaterial != null ? ghostMaterial : renderer.material);
            Color color = ghostMat.color;
            color.a = 0.5f; // 50% прозрачности
            ghostMat.color = color;
            // Важно: если материал не настроен на режим прозрачности, изменить рендеринг может потребоваться дополнительно
            renderer.material = ghostMat;
        }
    }

    // Проверка корректности позиции размещения
    // Здесь можно добавить логику проверки коллизий или ограничений по зонам
    private bool IsPlacementValid(Vector3 position)
    {
        // Пока возвращаем true. Можно, например, использовать Physics.OverlapSphere для проверки пересечений.
        return true;
    }
}
