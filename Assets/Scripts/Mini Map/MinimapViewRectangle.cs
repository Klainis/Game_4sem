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

        Vector3[] corners = new Vector3[4];
        corners[0] = ScreenToGround(mainCamera, new Vector3(0, 0, 0));                              // Bottom-left
        corners[1] = ScreenToGround(mainCamera, new Vector3(Screen.width, 0, 0));                   // Bottom-right
        corners[2] = ScreenToGround(mainCamera, new Vector3(Screen.width, Screen.height, 0));       // Top-right
        corners[3] = ScreenToGround(mainCamera, new Vector3(0, Screen.height, 0));                  // Top-left

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
