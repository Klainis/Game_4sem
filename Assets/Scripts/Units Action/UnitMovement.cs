using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    Unit unit;

    Animator animator;

    public LayerMask ground;
    public LayerMask attackable;
    public LayerMask clickable;

    public bool isCommandedToMove;
    public bool isFollowingTarget;
    private float lastMoveCommandTime;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();

        agent.speed = unit.moveSpeed;

        //animator.enabled = false;
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
                isFollowingTarget = false;

                animator.SetBool("isMoving", true);

                Debug.Log("Двигается" + isCommandedToMove);

                lastMoveCommandTime = Time.time;
                agent.transform.LookAt(hitGround.point);
                agent.SetDestination(hitGround.point);
            }
        }

        //if (isFollowingTarget || (agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance))
        if (agent.remainingDistance <= agent.stoppingDistance &&
                     (Time.time - lastMoveCommandTime > 0.1f))
        {
            isCommandedToMove = false;
            animator.SetBool("isMoving", false);
            Debug.Log(animator.GetBool("isMoving"));
        }
        else
        {
            //animator.SetBool("isMoving", true);
        }
    }
}
 