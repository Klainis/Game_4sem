using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;

    internal void TakeDamage(int damageAttack)
    {
        health -= damageAttack;
    }
}