using UnityEngine;
using UnityEngine.AI;

[AddComponentMenu("Building/Wall")]
[RequireComponent(typeof(NavMeshObstacle))]
public class Wall : BuildingBase
{
    [SerializeField] int layerHealth = 100;
    [SerializeField] Renderer visual;

    protected override void Awake()
    {
        base.Awake();
        if (!visual) visual = GetComponentInChildren<Renderer>();

        var obstacle = GetComponent<NavMeshObstacle>();
        obstacle.carving = true; // юниты перестраивают путь вокруг стены
    }

    /// <summary>
    /// Увеличить запас прочности при установке новой стены поверх существующей.
    /// </summary>
    public void AddLayer()
    {
        maxHealth += layerHealth;
        ModifyHealth(layerHealth);
        UpdateVisual();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (!visual) return;
        float t = Mathf.InverseLerp(0, maxHealth, CurrentHealth);
        visual.material.color = Color.Lerp(Color.red, Color.white, t);
    }
}