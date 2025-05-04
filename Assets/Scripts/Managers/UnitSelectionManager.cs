using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class UnitSelectionManager : MonoBehaviour
{
    // Start is called before he first frame update
    public static UnitSelectionManager Instance { get; set; }

    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitSelected = new List<GameObject>();

    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask attackable;
    public LayerMask buildClickable;

    private Color colorOFCursor = Color.green;

    public bool attackCursorVisible;

    private Camera cam;
    public bool boolTarget;

    //private EnemOutline enemOutline;

    //private GameObject hitFriendly;



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
        //enemOutline = GameObject.FindObjectOfType<EnemOutline>();
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
            //RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitGround, hitAttackable;
            bool isGroundHit = Physics.Raycast(ray, out hitGround, Mathf.Infinity, ground);
            bool isAttackableHit = Physics.Raycast(ray, out hitAttackable, Mathf.Infinity, attackable);
            bool isFriendlyHit = Physics.Raycast(ray, out hitAttackable, Mathf.Infinity, clickable);

            //Нажимаем на clickable oбъект
            if (isGroundHit && !isAttackableHit && !isFriendlyHit)
            {
                //groundMarker.transform.position = hit.point;

                var positionMarker = hitGround.point;

                GameObject movementCursor = GameObject.Find("ArrowMesh");
                MovementCursor mover = movementCursor.GetComponent<MovementCursor>();

                mover.AnimateOnPos(positionMarker, colorOFCursor);
                //groundMarker.SetActive(false);
                //groundMarker.SetActive(true);
            }
        }

        //Attack Target
        if (unitSelected.Count > 0 && AtleastOneOffensiveUnit(unitSelected))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            //Нажимаем на attackable oбъект
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, attackable))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 1f);
                UnitFollowingToTarget(hit, "enemy");
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                UnitFollowingToTarget(hit, "unit");
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildClickable))
            {
                UnitFollowingToTarget(hit, "build");
            }
            else
            {
                attackCursorVisible = false;
            }
        }
    }

    private void UnitFollowingToTarget(RaycastHit hit, string followTarget)
    {
        attackCursorVisible = true;

        if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log("Follow to " + followTarget);
            Transform target = hit.transform;

            foreach (GameObject unit in unitSelected)
            {
                if (unit.GetComponent<AttackController>())
                {
                    unit.GetComponent<AttackController>().targetToAttack = target;

                    if (unit.TryGetComponent<UnitMovement>(out var movement) && unit.GetComponent<AttackController>().targetToAttack != null)
                    {
                        
                        movement.isFollowingTarget = true;
                        
                        movement.isCommandedToMove = false; // Чтобы анимация движения работала
                        Debug.Log("Follow to " + followTarget + movement.isFollowingTarget + ";  To Move command " + movement.isCommandedToMove);
                    }
                }
            }
        }
    }

    private bool AtleastOneOffensiveUnit(List<GameObject> unitSelected)
    {
        foreach (GameObject unit in unitSelected)
        {
            if (unit.GetComponent<AttackController>())
            {
                return true;
            }
        }
        return false;
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

        //groundMarker.SetActive(false);
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
