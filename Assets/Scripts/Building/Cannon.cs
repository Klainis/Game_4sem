using UnityEngine;

[AddComponentMenu("Building/Cannon")]
[RequireComponent(typeof(SphereCollider))]
public class Cannon : BuildingBase
{
    [Header("References")]
    [SerializeField] RangeVisualizer range;              // визуализация радиуса
    [SerializeField] Transform turret;                    // поворотная часть пушки
    [SerializeField] Transform muzzle;                    // точка вылета снаряда
    
    [Header("Attack")]
    [SerializeField] int damage = 9;
    [SerializeField] float shootInterval = 2f;
    [SerializeField] float attackRange = 6f;
    [SerializeField] float rotationSpeed = 5f;           // скорость поворота
    [SerializeField] GameObject projectilePrefab;

    float timer;
    public Transform target;
    SphereCollider detection;
    bool isActive;
    bool isGhost;

    protected override void Awake()
    {
        base.Awake();
        
        // Поиск компонентов если не заданы
        if (!range)
            range = GetComponentInChildren<RangeVisualizer>(true);
        if (!turret)
            turret = transform.Find("Turret");
        if (!muzzle && turret)
            muzzle = turret.Find("Muzzle");

        // Проверяем наличие префаба снаряда
        if (!projectilePrefab)
        {
            Debug.LogError("[Cannon] Projectile prefab is not assigned!");
        }

        // Настройка коллайдера обнаружения
        detection = GetComponent<SphereCollider>();
        detection.isTrigger = true;
        detection.radius = attackRange;
        detection.enabled = false; // Отключаем коллайдер до активации

        // Скрываем визуализатор радиуса и отключаем стрельбу
        range?.Hide();
        isActive = false;
        isGhost = true;

        Debug.Log($"[Cannon] Awake: radius={attackRange}, damage={damage}, projectilePrefab={projectilePrefab != null}");
    }

    void Start()
    {
        // Проверяем, является ли объект призраком (в процессе размещения)
        var placementManager = FindObjectOfType<BuildingPlacementManager>();
        if (placementManager && placementManager.CurrentGhost == gameObject)
        {
            Debug.Log("[Cannon] Start: This is a ghost");
            return;
        }

        // Если это не призрак, активируем пушку
        isGhost = false;
        detection.enabled = true;
        Invoke(nameof(Activate), 0.5f);
        Debug.Log("[Cannon] Start: Real cannon initialized");
    }

    void Activate()
    {
        if (!isGhost)
        {
            isActive = true;
            Debug.Log("[Cannon] Activated");

            // Сразу ищем врагов в радиусе
            FindEnemiesInRange();
        }
    }

    void FindEnemiesInRange()
    {
        var colliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var col in colliders)
        {
            if (IsEnemy(col) && target == null)
            {
                target = col.transform;
                Debug.Log($"[Cannon] Found enemy: {col.name} at position {col.transform.position}");
                break;
            }
        }
    }

    void Update()
    {
        if (isGhost || !isActive) 
        { 
            timer = 0f; 
            return; 
        }

        // Если нет цели, попробуем найти
        if (!target)
        {
            FindEnemiesInRange();
            return;
        }

        // Поворот к цели
        if (turret)
        {
            Vector3 targetDir = (target.position - turret.position).normalized;
            targetDir.y = 0; // оставляем поворот только по горизонтали
            
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            turret.rotation = Quaternion.Slerp(turret.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Стрельба
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Shoot();
            timer = shootInterval;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!isGhost && isActive && IsEnemy(other) && target == null)
        {
            target = other.transform;
            //Debug.Log($"[Cannon] Enemy entered range: {other.name} at position {other.transform.position}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == target)
        {
            Debug.Log($"[Cannon] Enemy left range: {other.name}");
            target = null;
        }
    }

    bool IsEnemy(Collider col)
    {
        bool isEnemy = col.CompareTag("Enemy");
        if (isEnemy)
        {
            //Debug.Log($"[Cannon] Detected enemy: {col.name} at position {col.transform.position}");
        }
        return isEnemy;
    }

    void Shoot()
    {
        if (!target)
        {
            Debug.Log("[Cannon] Can't shoot: no target");
            return;
        }

        if (!projectilePrefab)
        {
            Debug.LogError("[Cannon] Can't shoot: projectilePrefab is missing!");
            return;
        }

        //Debug.Log($"[Cannon] Shooting at {target.name} from position {(muzzle ? muzzle.position : transform.position)}");

        // Снаряд
        Vector3 spawnPos = muzzle ? muzzle.position : transform.position + Vector3.up * 1.5f;
        var proj = Instantiate(projectilePrefab, spawnPos, muzzle ? muzzle.rotation : Quaternion.identity);
        
        if (proj.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.Init(target, damage);
            //Debug.Log($"[Cannon] Projectile initialized with target {target.name} and damage {damage}");
        }
        else
        {
            Debug.LogError("[Cannon] Projectile component not found on instantiated prefab!");
        }
    }

    // Показать/скрыть радиус при клике
    void OnMouseDown()
    {
        if (!isGhost && range)
            range.Toggle();
    }

    // Вызывается BuildingPlacementManager после успешного размещения
    public void OnPlaced()
    {
        Debug.Log("[Cannon] OnPlaced called");
        isGhost = false;
        detection.enabled = true;
        Invoke(nameof(Activate), 0.5f);
    }

    void OnDrawGizmos()
    {
        // Визуализация радиуса атаки в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
