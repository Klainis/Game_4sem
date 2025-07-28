using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MinimapViewRectangle : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float rectangleThickness = 2f;
    [SerializeField] float projectionHeight = 0f; // Y-уровень плоскости-проекции

    private Mesh mesh;
    private Vector3[] vertices = new Vector3[8];

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Minimap View Rectangle";

        mesh.vertices = vertices;
        mesh.triangles = new int[]
        {
            0,4,1, 1,4,5,
            1,5,2, 2,5,6,
            2,6,3, 3,6,7,
            3,7,0, 0,7,4
        };

        GetComponent<Renderer>().material.renderQueue = 5000;

    }

    void LateUpdate()
    {
        UpdateRectangle();
        //Debug.Log("Corners: " + vertices[0] + ", " + vertices[1]);
    }

    void UpdateRectangle()
    {
        if (mainCamera == null) return;

        // Получаем центр экрана
        Vector3 center = ScreenToGround(mainCamera, new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        // Вычисляем углы с компенсацией перспективы и поворотом на 90 градусов
        Vector3[] corners = new Vector3[4];
        float aspect = (float)Screen.width / Screen.height;
        float distance = Vector3.Distance(mainCamera.transform.position, center);
        float heightScale = distance * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float widthScale = heightScale * aspect;

        // Поворот на 90 градусов (меняем местами width и height с изменением знака)
        corners[0] = center + new Vector3(-heightScale, 0, -widthScale);
        corners[1] = center + new Vector3(-heightScale, 0, widthScale);
        corners[2] = center + new Vector3(heightScale, 0, widthScale);
        corners[3] = center + new Vector3(heightScale, 0, -widthScale);

        // Остальной код без изменений
        for (int i = 0; i < 4; i++)
        {
            Vector3 dir = (corners[(i + 1) % 4] - corners[i]).normalized;
            Vector3 offset = Vector3.Cross(dir, Vector3.up) * rectangleThickness * 0.5f;

            vertices[i] = transform.InverseTransformPoint(corners[i] - offset);
            vertices[i + 4] = transform.InverseTransformPoint(corners[i] + offset);
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    Vector3 ScreenToGround(Camera cam, Vector3 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, new Vector3(0, projectionHeight, 0));
        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }
}
