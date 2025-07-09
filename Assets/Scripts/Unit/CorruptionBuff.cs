using UnityEngine;

public class CorruptionBuff : MonoBehaviour
{
    private Unit unit;
    private float originalSpeed;
    private int originalDamage;
    private bool applied = false;
    private Renderer rend;
    private Color originalColor;

    [Header("Buff Settings")]
    public float speedMultiplier = 1.5f;
    public float damageMultiplier = 1.3f;
    public Color buffColor = new Color(0.3f, 0.1f, 0.4f, 1f); // тёмно-фиолетовый

    void Start()
    {
        unit = GetComponent<Unit>();
        if (unit != null && !applied)
        {
            originalSpeed = unit.moveSpeed;
            originalDamage = unit.unitDamage;
            unit.moveSpeed *= speedMultiplier;
            unit.unitDamage = Mathf.RoundToInt(unit.unitDamage * damageMultiplier);
            applied = true;
        }
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
            rend.material.color = buffColor;
        }
    }

    void OnDestroy()
    {
        if (unit != null && applied)
        {
            unit.moveSpeed = originalSpeed;
            unit.unitDamage = originalDamage;
        }
        if (rend != null)
        {
            rend.material.color = originalColor;
        }
    }
} 