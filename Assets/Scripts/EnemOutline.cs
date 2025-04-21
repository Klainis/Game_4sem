using UnityEngine;
public class EnemOutline : MonoBehaviour
{
    private UnitSelectionManager unitSelectionManager;

    private GameObject hitEnemy;

    private bool isVisible;
    public LayerMask attackable;

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
        //Debug.Log(isVisible);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, attackable) && isVisible)
        {
            hitEnemy = hit.collider.gameObject;

            TriggerEnemyIndicator(hitEnemy, isVisible);

            //Debug.Log(isVisible);
            //TriggerEnemyIndicator(hit.collider.gameObject, false);
        }
        else if (hitEnemy && !isVisible)
        {
            //Debug.Log(isVisible);
            TriggerEnemyIndicator(hitEnemy, isVisible);
        }
    }
    public void TriggerEnemyIndicator(GameObject unit, bool isVisible)
    {
        //Debug.Log(isVisible);

        Outline outline = unit.GetComponent<Outline>();

        if (outline != null)
        {
            //Debug.Log("Unit is be");
            outline.enabled = isVisible;
        }
    }
}
 