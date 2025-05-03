using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Считывает ввод игрока во время режима постройки
/// и транслирует его в события.  Никакой игровой логики -
/// только «какую кнопку нажали».
/// </summary>
public class PlacementInputHandler : MonoBehaviour
{
    // ► события, на которые подпишется BuildingPlacementManager
    public event Action RotateRequested;            // R
    public event Action CancelRequested;            // RMB | Esc
    public event Action<Vector3> PlaceRequested;    // LMB + точка мира

    [SerializeField] KeyCode rotateKey = KeyCode.R;

    Camera cam;

    void Awake() => cam = Camera.main;

    void Update()
    {
        if (Input.GetKeyDown(rotateKey))
            RotateRequested?.Invoke();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            CancelRequested?.Invoke();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
                PlaceRequested?.Invoke(hit.point);
        }
    }
}
