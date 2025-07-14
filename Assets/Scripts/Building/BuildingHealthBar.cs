using UnityEngine;

[RequireComponent(typeof(BuildingBase))]
public class BuildingHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthTracker healthTracker;   // Slider UI prefab в дочернем Canvas
    [SerializeField] private Vector3 barOffset = new Vector3(0, 3f, 0); // высота над зданием

    private BuildingBase building;

    void Awake()
    {
        building = GetComponent<BuildingBase>();
        if (!healthTracker)
        {
            healthTracker = GetComponentInChildren<HealthTracker>(true);
        }

        if (building != null)
        {
            building.OnDamaged += HandleDamaged;
            building.OnDestroyed += HandleDestroyed;
        }

        // Инициализация стартового значения
        if (healthTracker != null)
            healthTracker.UpdateSliderValue(building.MaxHealth, building.MaxHealth);
    }

    private void Update()
    {
        // Обновляем позицию бара над зданием
        if (healthTracker != null)
        {
            healthTracker.transform.position = transform.position + barOffset;
        }
    }

    private void HandleDamaged(int current, int max)
    {
        if (healthTracker != null)
        {
            healthTracker.UpdateSliderValue(current, max);
        }
    }

    private void HandleDestroyed()
    {
        if (healthTracker != null)
            Destroy(healthTracker.gameObject);
    }

    private void OnDestroy()
    {
        if (building != null)
        {
            building.OnDamaged -= HandleDamaged;
            building.OnDestroyed -= HandleDestroyed;
        }
    }
} 