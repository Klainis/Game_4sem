using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FriendlyOutline : MonoBehaviour
{
    private UnitSelectionManager unitSelectionManager;

    private GameObject hitFriendly;
    private List<GameObject> excludedObjects;

    private bool isVisible;

    public LayerMask clickable;
    public LayerMask buildsClickable;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        unitSelectionManager = GameObject.FindObjectOfType<UnitSelectionManager>();
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        isVisible = unitSelectionManager.attackCursorVisible;

        if (unitSelectionManager.unitSelected.Count > 0)
        {
            excludedObjects = unitSelectionManager.unitSelected;

            if ((Physics.Raycast(ray, out hit, Mathf.Infinity, clickable) && isVisible) ||
                Physics.Raycast(ray, out hit, Mathf.Infinity, buildsClickable) && isVisible)
            {
                hitFriendly = hit.collider.gameObject;

                if (!excludedObjects.Contains(hitFriendly))
                {
                    TriggerEnemyIndicator(hitFriendly, isVisible);
                }
            }
            else if (hitFriendly && !isVisible && !excludedObjects.Contains(hitFriendly))
            {
                TriggerEnemyIndicator(hitFriendly, isVisible);
            }
        }
    }
    public void TriggerEnemyIndicator(GameObject unit, bool isVisible)
    {
        Outline outline = unit.GetComponent<Outline>();

        if (outline != null)
        {
            //Debug.Log("Unit is be");
            outline.enabled = isVisible;
        }
    }
}
