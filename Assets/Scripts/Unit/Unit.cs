using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float unitHealth { get; private set; }

    public float unitMaxHealth;
    public int unitDamage;
    public float attackRange; //5
    public float attackRate; //2
    public int armor;
    public float moveSpeed = 3.5f;

    public HealthTracker healthTracker;

    AttackController attackController;

    void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);

        attackController = GetComponent<AttackController>();

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
            // �������� �������� � �������� �������
            Destroy(gameObject);
        }
    }

    internal void TakeDamage(int damageAttack)
    {
        var damageAttackAfterArmor = damageAttack - damageAttack * (armor / 100);
        Debug.Log(damageAttackAfterArmor);
        unitHealth -= damageAttackAfterArmor;
        UpdateHealthUI();
    }

    void Update()
    {
        // Проверка на скверну
        if (CorruptionGridManager.Instance != null)
        {
            bool isCorrupted = CorruptionGridManager.Instance.IsCorruptedCell(transform.position);
            var debuff = GetComponent<CorruptionDebuff>();
            if (isCorrupted && debuff == null)
            {
                gameObject.AddComponent<CorruptionDebuff>();
            }
            else if (!isCorrupted && debuff != null)
            {
                Destroy(debuff);
            }
        }
    }
}
