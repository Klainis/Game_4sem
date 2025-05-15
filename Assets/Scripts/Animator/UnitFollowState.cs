using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;
    NavMeshAgent agent;
    Unit unit;

    public float attackingDistance;

    //private float targetRadius;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
        unit = animator.transform.GetComponent<Unit>();

        attackingDistance = unit.attackRange;
        Debug.Log(attackingDistance);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Следует переходить в состояние Idle State?
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
            Debug.Log(animator.transform.GetComponent<UnitMovement>().isCommandedToMove);
        }
        else
        {
            Debug.Log(animator.transform.GetComponent<UnitMovement>().isCommandedToMove);
            //Following если нет других комманд 
            if (animator.transform.GetComponent<UnitMovement>() != null &&
                animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
            {
                //Следование за врагом
                agent.SetDestination(attackController.targetToAttack.position);
                animator.transform.LookAt(attackController.targetToAttack);
                Debug.Log("Enemy following");

                if (attackController.targetToAttack.CompareTag("Enemy") 
                    && !animator.transform.CompareTag("Unit Monk Doctor") 
                    && attackController.isPlayer)
                {
                    float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

                    if (distanceFromTarget <= attackingDistance)
                    {
                        Debug.Log("Unit is Attacking!");
                        agent.SetDestination(animator.transform.position);
                        //agent.isStopped = true;
                            animator.SetBool("isAttacking", true);
                    }
                }
                else if (attackController.targetToAttack.CompareTag("Friendly") && !attackController.isPlayer)
                {
                    float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

                    if (distanceFromTarget <= attackingDistance)
                    {
                        Debug.Log("Enemy is Attacking!");
                        agent.SetDestination(animator.transform.position);
                        animator.SetBool("isAttacking", true);
                    }
                }
                else if (attackController.targetToAttack.CompareTag("Friendly") 
                    && animator.transform.CompareTag("Unit Monk Doctor") 
                    && attackController.isPlayer)
                {
                    float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

                    if (distanceFromTarget <= attackingDistance)
                    {
                        Debug.Log("Enemy is Attacking!");
                        agent.SetDestination(animator.transform.position);
                        animator.SetBool("isHealing", true);
                    }
                }

            }
            else if (animator.transform.GetComponent<UnitMovement>() != null && 
                animator.transform.GetComponent<UnitMovement>().isCommandedToMove == true)//Если получил команду на перемещение, убираем таргет
            {
                attackController.targetToAttack = null;
            }
        }

    }

    //// OnStateExit is called when a transition ends and the state machine finishes evaluating this state

}
