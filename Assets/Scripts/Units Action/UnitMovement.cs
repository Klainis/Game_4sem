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

        if (agent != null)
        {
            agent.speed = unit.moveSpeed;
            
            // Убедимся что агент на поверхности NavMesh
            if (!agent.isOnNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
                else
                {
                    Debug.LogWarning($"Не удалось разместить {gameObject.name} на NavMesh!");
                }
            }
        }
        else
        {
            Debug.LogError($"На объекте {gameObject.name} отсутствует компонент NavMeshAgent!");
        }
    }

    private void Update()
    {
        if (!agent || !agent.isOnNavMesh) return;

        if (Input.GetMouseButtonDown(1))
        {
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
                lastMoveCommandTime = Time.time;

                // Поворачиваем юнита в сторону движения
                Vector3 direction = (hitGround.point - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                agent.SetDestination(hitGround.point);
            }
        }

        // Проверяем, достиг ли агент цели
        if (agent.pathStatus != NavMeshPathStatus.PathInvalid && 
            !agent.pathPending && 
            agent.remainingDistance <= agent.stoppingDistance &&
            Time.time - lastMoveCommandTime > 0.1f)
        {
            isCommandedToMove = false;
            animator.SetBool("isMoving", false);
        }
    }
}
 