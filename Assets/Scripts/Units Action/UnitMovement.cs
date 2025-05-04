using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    public LayerMask ground;

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
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 0.1f);
                
                isCommandedToMove = true;
                Debug.Log("Двигается" + isCommandedToMove);
                isCommandedToMove = false;
                isFollowingTarget = false;
                agent.SetDestination(hit.point);
                //Debug.Log(hit.point);
            }
        }

        //if (!isFollowingTarget || (agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance))
        //{
        //    //isCommandedToMove = false;
        //}
    }
}
 