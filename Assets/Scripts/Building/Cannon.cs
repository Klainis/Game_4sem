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
    Transform target;
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

        // Настройка коллайдера обнаружения
        detection = GetComponent<SphereCollider>();
        detection.isTrigger = true;
        detection.radius = attackRange;
        detection.enabled = false; // Отключаем коллайдер до активации

        // Скрываем визуализатор радиуса и отключаем стрельбу
        range?.Hide();
        isActive = false;
        isGhost = true;

        Debug.Log($"[Cannon] Awake: radius={attackRange}, damage={damage}");
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
                Debug.Log($"[Cannon] Found enemy: {col.name}");
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

    void OnTriggerEnter(Collider other)
    {
        if (!isGhost && isActive && IsEnemy(other) && target == null)
        {
            target = other.transform;
            Debug.Log($"[Cannon] Enemy entered range: {other.name}");
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
            Debug.Log($"[Cannon] Detected enemy: {col.name}");
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

        Debug.Log($"[Cannon] Shooting at {target.name}");

        // Мгновенный урон
        if (!projectilePrefab)
        {
            target.GetComponent<Unit>()?.TakeDamage(damage);
            return;
        }

        // Снаряд
        Vector3 spawnPos = muzzle ? muzzle.position : transform.position + Vector3.up * 1.5f;
        var proj = Instantiate(projectilePrefab, spawnPos, muzzle ? muzzle.rotation : Quaternion.identity);
        proj.GetComponent<Projectile>().Init(target, damage);
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
