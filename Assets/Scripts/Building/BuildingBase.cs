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
    public int CurrentHealth { get; private set; }

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
        if (CurrentHealth <= 0)
        {
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