using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    // Start is called before he first frame update
    public static UnitSelectionManager Instance { get; set; }

    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitSelected = new List<GameObject>();

    public GameObject groundMarker;
    public LayerMask clickable;
    public LayerMask ground;
    private Camera cam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            //�������� �� clickable o�����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    MultiSelect(hit.collider.gameObject);
                }
                else
                {
                    SelectByClicking(hit.collider.gameObject);
                }
            }
            else //�� �������
            {
                if (Input.GetKey(KeyCode.LeftControl) == false)
                {
                    DeselectAll();
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && unitSelected.Count > 0)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            //�������� �� clickable o�����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                groundMarker.transform.position = hit.point;

                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
            }
        }
    }

    private void MultiSelect(GameObject unit)
    {
        if (unitSelected.Contains(unit) == false)
        {
            unitSelected.Add(unit);
            SelectUnit(unit, true);
        }
        else
        {
            SelectUnit(unit, false);
            unitSelected.Remove(unit);
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in unitSelected)
        {
            SelectUnit(unit, false);
        }

        groundMarker.SetActive(false);
        unitSelected.Clear();
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();

        unitSelected.Add(unit);

        SelectUnit(unit, true);
    }

    private void SelectUnit(GameObject unit, bool isSelected)
    {
        TriggerSelectionIndicator(unit, isSelected);
        EnableUnitMovement(unit, isSelected);
    }
    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        //unit.transform.GetChild(0).gameObject.SetActive(isVisivble);
        Outline outline = unit.GetComponent<Outline>();

        if (outline != null)
        {
            outline.enabled = isVisible;
        }
    }

    internal void DragSelect(GameObject unit)
    {
        if (unitSelected.Contains(unit) == false)
        {
            unitSelected.Add(unit);
            SelectUnit(unit, true);

        }
    }
}
