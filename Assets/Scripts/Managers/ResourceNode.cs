using System;
using UnityEngine;

/// <summary>
/// Источник ресурса (золота). Содержит остаток и событие,
/// когда он полностью исчерпан.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ResourceNode : MonoBehaviour
{
    [Header("Amount (units of gold)")]
    [SerializeField] int totalAmount = 1000;

    public  int  Remaining { get; private set; }
    public bool IsDepleted => Remaining <= 0;

    public event Action<ResourceNode> OnDepleted;

    void Awake() => Remaining = totalAmount;

    /// <summary>
    /// Уменьшает остаток и возвращает фактически добытое количество.
    /// </summary>
    public int Extract(int want)
    {
        if (IsDepleted) return 0;

        int taken = Mathf.Min(want, Remaining);
        Remaining -= taken;
        if (IsDepleted) OnDepleted?.Invoke(this);
        return taken;
    }
}
