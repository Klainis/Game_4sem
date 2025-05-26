using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;
    NavMeshAgent agent;
    Unit unit;
    UnitMovement unitMovement;

    public float attackingDistance;

    //private float targetRadius;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
        unit = animator.transform.GetComponent<Unit>();
        unitMovement = animator.transform.GetComponent<UnitMovement>();

        attackingDistance = unit.attackRange;
        //Debug.Log(attackingDistance);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log($"State: {animator.GetCurrentAnimatorStateInfo(0).fullPathHash} | " +
        //      $"Moving: {animator.GetBool("isMoving")} | " +
        //      $"Velocity: {agent.velocity.magnitude}");

        //Следует переходить в состояние Idle State?
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
            //Debug.Log(unitMovement.isCommandedToMove);
        }
        else
        {
            //Following если нет других комманд 
            if (unitMovement != null &&
                unitMovement.isCommandedToMove == false)
            {
                unitMovement.isFollowingTarget = true;

                //if (NavMesh.SamplePosition(animator.transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                //{
                //    //animator.enabled = false;
                //    //animator.applyRootMotion = false;
                //    //animator.SetBool("isMoving", true);
                //    agent.SetDestination(hit.position);
                //    Debug.Log("ДВИГАЕТСЯ КО ВРАГУ");
                //    //agent.SetDestination(new Vector3(5, 0, 5));
                //}
                //else
                //{
                //    Debug.LogError("Точка не на NavMesh или слишком далеко!");
                //}
                animator.SetBool("isMoving", true);
                if (attackController.targetToAttack != null)
                {
                    Debug.Log("Идем ко врагу");
                    agent.SetDestination(attackController.targetToAttack.position);

                }

                animator.transform.LookAt(attackController.targetToAttack);
                //Debug.Log("Following");

                if (attackController.targetToAttack.CompareTag("Enemy")
                    && !animator.transform.CompareTag("Unit Monk Doctor")
                    && attackController.isPlayer) // Следование и атака для обычных юнитов
                {

                    float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

                    if (distanceFromTarget <= attackingDistance)
                    {
                        Debug.Log("Unit is Attacking!");
                        agent.SetDestination(animator.transform.position);
                        animator.SetBool("isAttacking", true);
                    }
                }
                else if (attackController.targetToAttack.CompareTag("Friendly") && !attackController.isPlayer) //Следование и атака для врагов
                {

                    float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

                    if (distanceFromTarget <= attackingDistance)
                    {
                        Debug.Log("Enemy is Attacking!");
                        agent.SetDestination(animator.transform.position);
                        animator.SetBool("isAttacking", true);
                    }
                }
                else if (attackController.targetToAttack.CompareTag("Friendly") // Следование и хил для доктора
                    && animator.transform.CompareTag("Unit Monk Doctor")
                    && attackController.isPlayer)
                {

                    float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

                    if (distanceFromTarget <= attackingDistance)
                    {
                        Debug.Log("Doctor is Healing!");
                        agent.SetDestination(animator.transform.position);
                        animator.SetBool("isHealing", true);
                    }
                }

            }

            else if (unitMovement != null &&
                unitMovement.isCommandedToMove == true)//Если получил команду на перемещение, убираем таргет
            {
                Debug.Log($"unitMovement: {unitMovement}, unitMovement.isCommandedToMove: {unitMovement.isCommandedToMove}");
                Debug.Log("УШЛИ ОТ ВРАГА");
                attackController.targetToAttack = null;
                animator.SetBool("isAttacking", false);
            }
        }
        
    }
}
