using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Building/EnemySpawnBuilding")]
public class EnemySpawnBuilding : BuildingBase
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f; // Задержка между спавнами
    [SerializeField] private GameObject[] enemyPrefabs; // Кого спавнить
    [SerializeField] private int maxUnits = 3; // Ограничение на количество живых врагов
    [SerializeField] private float spawnRadius = 2f; // Радиус появления вокруг здания
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject spawnEffect; // Эффект появления
    [SerializeField] private ParticleSystem spawnParticles; // Частицы спавна
    [SerializeField] private AudioClip spawnSound; // Звук спавна
    
    [Header("References")]
    [SerializeField] private Transform spawnPoint; // Точка спавна (если не задана, используется позиция здания)
    
    private List<GameObject> spawnedUnits = new List<GameObject>();
    private Coroutine spawnRoutine;
    private AudioSource audioSource;
    
    protected override void Awake()
    {
        base.Awake();
        maxHealth = 200; // Базовое здоровье для вражеского здания
        
        // Получаем AudioSource или создаем
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && spawnSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Если точка спавна не задана, используем позицию здания
        if (spawnPoint == null)
            spawnPoint = transform;
    }
    
    private void Start()
    {
        // Начинаем спавн через небольшую задержку
        StartSpawning();
        
        // Заражаем область вокруг вражеского здания
        if (CorruptionGridManager.Instance != null)
        {
            Vector2Int gridPos = CorruptionGridManager.Instance.WorldToGrid(transform.position);
            CorruptionGridManager.Instance.CorruptArea(gridPos, 2); // Заражаем область 2x2
        }
    }
    
    public void StartSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }
    
    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }
    
    private IEnumerator SpawnRoutine()
    {
        // Начальная задержка
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            
            // Очищаем список от уничтоженных врагов
            CleanUpDestroyedUnits();
            
            // Проверяем, не превышен ли лимит
            if (spawnedUnits.Count < maxUnits && enemyPrefabs.Length > 0)
            {
                SpawnEnemy();
            }
        }
    }
    
    private void SpawnEnemy()
    {
        // Выбираем случайного врага из списка
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        
        // Определяем позицию спавна
        Vector3 spawnPos = GetRandomSpawnPosition();
        
        // Проверяем, что позиция свободна
        if (!IsPositionClear(spawnPos))
        {
            // Пробуем найти другую позицию
            for (int attempts = 0; attempts < 5; attempts++)
            {
                spawnPos = GetRandomSpawnPosition();
                if (IsPositionClear(spawnPos))
                    break;
            }
        }
        
        // Создаем врага
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        // Добавляем в список отслеживаемых юнитов
        spawnedUnits.Add(enemy);
        
        // Показываем визуальный эффект
        ShowSpawnEffect(spawnPos);
        
        // Воспроизводим звук
        PlaySpawnSound();
        
        Debug.Log($"[EnemySpawnBuilding] Spawned {enemyPrefab.name} at {spawnPos}");
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = spawnPoint.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        
        // Проецируем на поверхность (если нужно)
        if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
        {
            spawnPos.y = hit.point.y;
        }
        
        return spawnPos;
    }
    
    private bool IsPositionClear(Vector3 position)
    {
        // Проверяем, нет ли препятствий в радиусе 1 метра
        Collider[] colliders = Physics.OverlapSphere(position, 1f);
        foreach (var collider in colliders)
        {
            // Игнорируем триггеры и само здание
            if (collider.isTrigger || collider.transform == transform)
                continue;
                
            // Если есть другие объекты - позиция занята
            if (collider.CompareTag("Building") || collider.CompareTag("Friendly") || collider.CompareTag("Enemy"))
                return false;
        }
        return true;
    }
    
    private void ShowSpawnEffect(Vector3 position)
    {
        // Визуальный эффект появления
        if (spawnEffect != null)
        {
            GameObject effect = Instantiate(spawnEffect, position, Quaternion.identity);
            Destroy(effect, 3f); // Удаляем через 3 секунды
        }
        
        // Частицы
        if (spawnParticles != null)
        {
            spawnParticles.transform.position = position;
            spawnParticles.Play();
        }
    }
    
    private void PlaySpawnSound()
    {
        if (audioSource != null && spawnSound != null)
        {
            audioSource.clip = spawnSound;
            audioSource.Play();
        }
    }
    
    private void CleanUpDestroyedUnits()
    {
        // Удаляем из списка уничтоженных врагов
        spawnedUnits.RemoveAll(unit => unit == null);
    }
    
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        
        // При уничтожении здания останавливаем спавн
        if (CurrentHealth <= 0)
        {
            StopSpawning();
        }
    }
    
    // Информация для отладки
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPoint.position, 1f); // Радиус проверки свободного места
        }
    }
    
    // Публичные методы для внешнего управления
    public int GetSpawnedUnitsCount() => spawnedUnits.Count;
    public int GetMaxUnits() => maxUnits;
    public float GetSpawnInterval() => spawnInterval;
    
    public void SetSpawnInterval(float newInterval)
    {
        spawnInterval = newInterval;
    }
    
    public void SetMaxUnits(int newMax)
    {
        maxUnits = newMax;
    }
} 