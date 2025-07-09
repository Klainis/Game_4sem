using UnityEngine;

public class CorruptionDebuff : MonoBehaviour
{
    private Unit unit;
    private float originalSpeed;
    private bool applied = false;
    private Renderer rend;
    private Color originalColor;

    [Header("Debuff Settings")]
    public float speedMultiplier = 0.5f;
    public Color debuffColor = new Color(0.6f, 0.2f, 0.8f, 1f); // фиолетовый

    void Start()
    {
        unit = GetComponent<Unit>();
        if (unit != null && !applied)
        {
            originalSpeed = unit.moveSpeed;
            unit.moveSpeed *= speedMultiplier;
            applied = true;
        }
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
            rend.material.color = debuffColor;
        }
    }

    void OnDestroy()
    {
        if (unit != null && applied)
        {
            unit.moveSpeed = originalSpeed;
        }
        if (rend != null)
        {
            rend.material.color = originalColor;
        }
    }
} 