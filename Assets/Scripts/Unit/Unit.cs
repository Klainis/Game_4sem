using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private float unitHealth;

    public float unitMaxHealth;
    public int unitDamage;
    public float attackRange; //5
    public float attackRate; //2
    public int armor;
    public float moveSpeed = 3.5f;

    public HealthTracker healthTracker;

    void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);

        unitHealth = unitMaxHealth;
        UpdateHealthUI();

    }

    private void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if (unitHealth <=0)
        { 
            // Добавить анимацию и звуковые эффекты
            Destroy(gameObject);
        }
    }

    internal void TakeDamage(int damageAttack)
    {
        unitHealth -= damageAttack;
        UpdateHealthUI();
    }
}
