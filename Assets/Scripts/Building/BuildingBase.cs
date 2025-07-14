using UnityEngine;

/// <summary>
/// Абстрактный класс для всех разрушаемых построек.
/// Хранит очки прочности и базовую логику получения урона.
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class BuildingBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    public event System.Action<int,int> OnDamaged;   // current, max
    public event System.Action OnDestroyed;

    protected virtual void Awake()
    {
        CurrentHealth = maxHealth;
    }

    /// <summary>
    /// Получить урон и разрушить объект при HP <= 0.
    /// </summary>
    public virtual void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        OnDamaged?.Invoke(CurrentHealth, maxHealth);
        if (CurrentHealth <= 0)
        {
            OnDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Protected method to modify health value, available to derived classes.
    /// </summary>
    protected void ModifyHealth(int amount)
    {
        CurrentHealth += amount;
    }
}