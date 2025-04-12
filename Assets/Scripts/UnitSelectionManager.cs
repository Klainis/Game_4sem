using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    // Start is called before he first frame update
    public static UnitSelectionManager Instance { get; set; }

    public List<GameObject> allUnitList = new List<GameObject>();
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

            //Нажимаем на clickable oбъект
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
            else //Не кликаем
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

            //Нажимаем на clickable oбъект
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
            TriggerSelectionIndicator(unit, true);
            EnableUnitMovement(unit, true);
        }
        else
        {
            EnableUnitMovement(unit, false);
            TriggerSelectionIndicator(unit, false);
            unitSelected.Remove(unit);
        }
    }

    private void DeselectAll()
    {
        foreach (var unit in unitSelected)
        {
            EnableUnitMovement(unit, false);
            TriggerSelectionIndicator(unit, false);
        }

        groundMarker.SetActive(false);
        unitSelected.Clear();
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();

        unitSelected.Add(unit);

        TriggerSelectionIndicator(unit, true);
        EnableUnitMovement(unit, true);
    }

    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisivble)
    {
        unit.transform.GetChild(0).gameObject.SetActive(isVisivble);
    }

}
