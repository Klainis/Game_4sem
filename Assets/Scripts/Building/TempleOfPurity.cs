using UnityEngine;
using System.Collections;

[AddComponentMenu("Building/TempleOfPurity")]
public class TempleOfPurity : BuildingBase
{
    [Header("Cleansing Settings")]
    [SerializeField] private float cleansingRadius = 5f;
    [SerializeField] private float cleansingInterval = 3f;
    [SerializeField] private int cleansingStrength = 1; // радиус очистки в клетках

    private Coroutine cleansingRoutine;

    protected override void Awake()
    {
        base.Awake();
        maxHealth = 150; // Прочность храма
    }

    public void OnPlaced()
    {
        // Начинаем очистку после размещения
        if (cleansingRoutine != null)
            StopCoroutine(cleansingRoutine);
        cleansingRoutine = StartCoroutine(CleansingRoutine());
    }

    private IEnumerator CleansingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(cleansingInterval);
            
            if (CorruptionGridManager.Instance != null)
            {
                Vector2Int gridPos = CorruptionGridManager.Instance.WorldToGrid(transform.position);
                CorruptionGridManager.Instance.CleanArea(gridPos, cleansingStrength);
            }
        }
    }

    private void OnDestroy()
    {
        if (cleansingRoutine != null)
            StopCoroutine(cleansingRoutine);
    }
} 