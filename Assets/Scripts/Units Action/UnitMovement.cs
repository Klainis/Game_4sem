using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    public LayerMask ground;
    public LayerMask attackable;
    public LayerMask clickable;

    public bool isCommandedToMove;
    public bool isFollowingTarget;
    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitGround, hitAttackable;
            bool isGroundHit = Physics.Raycast(ray, out hitGround, Mathf.Infinity, ground);
            bool isAttackableHit = Physics.Raycast(ray, out hitAttackable, Mathf.Infinity, attackable);
            bool isFriendlyHit = Physics.Raycast(ray, out hitAttackable, Mathf.Infinity, clickable);

            if (isGroundHit && !isAttackableHit && !isFriendlyHit)
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.green, 1f);
                
                isCommandedToMove = true;

                Debug.Log("Двигается" + isCommandedToMove);
                isFollowingTarget = false;
                agent.SetDestination(hitGround.point);
            }
        }

        if (isFollowingTarget || (agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance))
        {
            isCommandedToMove = false;
        }
    }
}
 