//using UnityEngine;

//public class Swordsman : Unit
//{
//    [Header("Swordsman Specific")]
//    [Range(0, 1)] public float blockChance = 0.2f;

//    protected override void Awake()
//    {
//        base.Awake();
//        unitType = UnitType.Melee;
//        unitSize = UnitSize.Medium;
//        damage = 15;
//        attackRange = 1.5f;
//        attackRate = 1.5f;
//        armor = 5;
//    }

//    public override void TakeDamage(int amount)
//    {
//        if (Random.value < blockChance)
//        {
//            amount = Mathf.RoundToInt(amount * 0.5f); // Блокирует 50% урона
//            Debug.Log("Swordsman blocked attack!");
//        }
//        base.TakeDamage(amount);
//    }
//}
