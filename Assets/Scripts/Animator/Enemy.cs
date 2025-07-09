using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;

    void Update()
    {
        // Проверка на скверну для врагов (усиление в заражённой зоне)
        if (CorruptionGridManager.Instance != null)
        {
            bool isCorrupted = CorruptionGridManager.Instance.IsCorruptedCell(transform.position);
            var buff = GetComponent<CorruptionBuff>();
            if (isCorrupted && buff == null)
            {
                gameObject.AddComponent<CorruptionBuff>();
            }
            else if (!isCorrupted && buff != null)
            {
                Destroy(buff);
            }
        }
    }

    internal void TakeDamage(int damageAttack)
    {
        health -= damageAttack;
    }
}