using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;

    NavMeshAgent agent;
    public float attackingDistance = 1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Следует переходить в состояние Idle State?
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
        }
        else
        {
            //Following если нет других комманд 
            if (animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
            {
                //Следование за врагом
                agent.SetDestination(attackController.targetToAttack.position);
                animator.transform.LookAt(attackController.targetToAttack);

                //Следует переходить в состояние Attacking State?
                //float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

                //if (distanceFromTarget < attackingDistance)
                //{
                //    agent.SetDestination(animator.transform.position);
                //    animator.SetBool("isAttackig", true);
                //}
            }
        }

    }

    //// OnStateExit is called when a transition ends and the state machine finishes evaluating this state

}
