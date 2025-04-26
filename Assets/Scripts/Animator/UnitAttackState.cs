using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    AttackController attackController;
    //UnitFollowState unitFollowState;

    public float stopAttackingDistance = 5.2f;


    private float attackRate = 2f;
    private float attackTimer = 1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();
        //unitFollowState = animator.GetBehaviour<UnitFollowState>();

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack != null &&
            animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
        {
            LookAtTarget();
            //agent.stoppingDistance = attackController.targetToAttack.GetComponent<Collider>().bounds.extents.magnitude + 5.2f;
            //stopAttackingDistance = agent.stoppingDistance;

            agent.SetDestination(attackController.targetToAttack.position);

            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = 1f / attackRate;
            
            }  
            else
            {
                attackTimer -= Time.deltaTime;
            }



            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

            if (distanceFromTarget > stopAttackingDistance || attackController.targetToAttack == null)
            {
                agent.SetDestination(animator.transform.position);
                animator.SetBool("isAttacking", false); // Move to Follow State
            }

        }


    }

    private void Attack()
    {
        var damageAttack = attackController.unitDamage;
        
        attackController.targetToAttack.GetComponent<Unit>().TakeDamage(damageAttack);
    }

    private void LookAtTarget()
    {
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}