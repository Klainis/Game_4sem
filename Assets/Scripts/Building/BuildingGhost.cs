using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Компонент для управления призрачными версиями зданий во время размещения.
/// Ghost-объекты должны содержать только визуальные компоненты и этот скрипт.
/// </summary>
public class BuildingGhost : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private Color validColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;
    [SerializeField] private float ghostAlpha = 0.5f;

    private Renderer[] ghostRenderers;
    private RangeVisualizer rangeVisualizer;
    private BuildingPlacementManager placementManager;

    void Awake()
    {
        // Убираем все ненужные компоненты для ghost-версии
        RemoveUnnecessaryComponents();
        
        // Получаем все рендереры для изменения материалов
        ghostRenderers = GetComponentsInChildren<Renderer>();
        
        // Ищем RangeVisualizer
        rangeVisualizer = GetComponentInChildren<RangeVisualizer>();
        
        // Меняем слой объекта и всех дочерних объектов, чтобы система тумана войны игнорировала ghost-здания
        int ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");
        SetLayerRecursively(gameObject, ignoreLayer);
        Debug.Log($"[BuildingGhost] Changed layer to {ignoreLayer} (Ignore Raycast) for ghost {gameObject.name}");
        
        // Настраиваем призрачный вид
        SetupGhostAppearance();
        
        Debug.Log($"[BuildingGhost] Ghost created for {gameObject.name}");
    }

    void Start()
    {
        // Находим placement manager
        placementManager = FindObjectOfType<BuildingPlacementManager>();
        
        // Показываем радиус, если есть RangeVisualizer
        if (rangeVisualizer != null)
        {
            rangeVisualizer.Show();
        }
    }

    /// <summary>
    /// Удаляет все компоненты, которые не нужны ghost-объекту
    /// </summary>
    private void RemoveUnnecessaryComponents()
    {
        // Удаляем коллайдеры (кроме тех, что нужны для проверки размещения)
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            // Оставляем только BoxCollider для проверки размещения
            if (!(col is BoxCollider))
            {
                DestroyImmediate(col);
            }
            else
            {
                // Делаем BoxCollider триггером, чтобы он не мешал
                col.isTrigger = true;
            }
        }

        // Удаляем NavMeshAgent
        var navAgent = GetComponent<NavMeshAgent>();
        if (navAgent) DestroyImmediate(navAgent);

        // Удаляем NavMeshObstacle
        var navObstacle = GetComponent<NavMeshObstacle>();
        if (navObstacle) DestroyImmediate(navObstacle);

        // Удаляем все строительные скрипты с учетом зависимостей
        SafeDestroyComponent<HealthTracker>();
        SafeDestroyComponent<Barrack>();
        SafeDestroyComponent<ProductionBuilding>();
        SafeDestroyComponent<GoldMine>();
        SafeDestroyComponent<Cannon>();
        SafeDestroyComponent<BuildingBase>();

        var wall = GetComponent<Wall>();
        if (wall) DestroyImmediate(wall);

        var templeOfPurity = GetComponent<TempleOfPurity>();
        if (templeOfPurity) DestroyImmediate(templeOfPurity);

        var enemySpawnBuilding = GetComponent<EnemySpawnBuilding>();
        if (enemySpawnBuilding) DestroyImmediate(enemySpawnBuilding);

        // Удаляем AI компоненты
        var unitComponents = GetComponentsInChildren<MonoBehaviour>();
        foreach (var comp in unitComponents)
        {
            // Удаляем компоненты, связанные с логикой игры
            if (comp.GetType().Name.Contains("AI") ||
                comp.GetType().Name.Contains("Health") ||
                comp.GetType().Name.Contains("Attack") ||
                comp.GetType().Name.Contains("Movement") ||
                comp.GetType().Name.Contains("Vision") ||
                comp.GetType().Name.Contains("Production"))
            {
                DestroyImmediate(comp);
            }
        }

        // Удаляем аудио источники
        var audioSources = GetComponentsInChildren<AudioSource>();
        foreach (var audio in audioSources)
        {
            DestroyImmediate(audio);
        }

        // Удаляем системы частиц
        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var particles in particleSystems)
        {
            DestroyImmediate(particles);
        }

        // Удаляем компонент тумана войны - ghost-здания не должны развеивать туман
        var fogVisibilities = GetComponentsInChildren<FogOfWarVisibility>();
        foreach (var fogVisibility in fogVisibilities)
        {
            if (fogVisibility != null)
            {
                DestroyImmediate(fogVisibility);
                Debug.Log($"[BuildingGhost] Removed FogOfWarVisibility from ghost {gameObject.name}");
            }
        }

        Debug.Log($"[BuildingGhost] Removed unnecessary components from {gameObject.name}");
    }

    /// <summary>
    /// Настраивает призрачный внешний вид
    /// </summary>
    private void SetupGhostAppearance()
    {
        foreach (var renderer in ghostRenderers)
        {
            if (renderer == null) continue;

            // Создаем новый материал на основе ghost материала или оригинального
            Material newMaterial;
            if (ghostMaterial != null)
            {
                newMaterial = new Material(ghostMaterial);
            }
            else
            {
                newMaterial = new Material(renderer.material);
            }

            // Устанавливаем прозрачность
            SetMaterialTransparent(newMaterial);
            Color color = newMaterial.color;
            color.a = ghostAlpha;
            newMaterial.color = color;

            renderer.material = newMaterial;
        }
    }

    /// <summary>
    /// Устанавливает цвет ghost-объекта в зависимости от валидности размещения
    /// </summary>
    public void SetValidationColor(bool isValid)
    {
        Color targetColor = isValid ? validColor : invalidColor;
        targetColor.a = ghostAlpha;

        foreach (var renderer in ghostRenderers)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = targetColor;
            }
        }
    }

    /// <summary>
    /// Поворачивает ghost-объект
    /// </summary>
    public void Rotate(float angle)
    {
        transform.Rotate(Vector3.up, angle, Space.World);
        Debug.Log($"[BuildingGhost] Rotated {gameObject.name} by {angle} degrees");
    }

    /// <summary>
    /// Устанавливает позицию ghost-объекта
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    /// <summary>
    /// Получает размер коллайдера для проверки размещения
    /// </summary>
    public Vector3 GetColliderSize()
    {
        var boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            return Vector3.Scale(boxCollider.size, transform.localScale);
        }

        // Fallback размер
        return new Vector3(1.1f, 1.5f, 1.1f);
    }

    /// <summary>
    /// Делает материал прозрачным
    /// </summary>
    private void SetMaterialTransparent(Material material)
    {
        // Устанавливаем режим рендеринга в Transparent
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Ghost должен писать глубину, чтобы туман войны перекрывал его
        material.SetInt("_ZWrite", 1);
        // Немного понижаем renderQueue, чтобы гарантировать отрисовку до Fog (Transparent+100 = 3100)
        material.renderQueue = 2999;
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    }

    /// <summary>
    /// Скрывает или показывает радиус визуализатора
    /// </summary>
    public void SetRangeVisualizerVisible(bool visible)
    {
        if (rangeVisualizer != null)
        {
            if (visible)
                rangeVisualizer.Show();
            else
                rangeVisualizer.Hide();
        }
    }

    void OnDestroy()
    {
        // Скрываем радиус при уничтожении
        if (rangeVisualizer != null)
        {
            rangeVisualizer.Hide();
        }

        Debug.Log($"[BuildingGhost] Ghost destroyed for {gameObject.name}");
    }

    void OnDrawGizmos()
    {
        // Визуализация размера коллайдера в редакторе
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        
        var size = GetColliderSize();
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
    
    /// <summary>
    /// Рекурсивно устанавливает слой для объекта и всех его дочерних объектов
    /// </summary>
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        
        obj.layer = newLayer;
        
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    /// <summary>
    /// Безопасно удаляет компонент с обработкой ошибок
    /// </summary>
    private void SafeDestroyComponent<T>() where T : Component
    {
        try
        {
            var component = GetComponent<T>();
            if (component) 
            {
                DestroyImmediate(component);
                Debug.Log($"[BuildingGhost] Removed {typeof(T).Name} component");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[BuildingGhost] Failed to remove {typeof(T).Name}: {e.Message}");
        }
    }
} 