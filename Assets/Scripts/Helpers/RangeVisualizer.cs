using UnityEngine;

/// <summary>
/// Рисует плоский круг LineRenderer-ом. Показывается/скрывается по команде.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class RangeVisualizer : MonoBehaviour
{
    [SerializeField] float radius = 3f;
    [SerializeField] int   segments = 60;

    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        BuildCircle();
    }

    public void Show  () => gameObject.SetActive(true);
    public void Hide  () => gameObject.SetActive(false);
    public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
    public float RadiusWorld => radius * transform.lossyScale.x;   // добавьте


    void BuildCircle()
    {
        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float a = 2 * Mathf.PI * i / segments;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, 0.05f, Mathf.Sin(a) * radius));
        }
    }
}
