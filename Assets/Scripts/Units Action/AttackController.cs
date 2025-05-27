using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    Animator animator;
    //UnitFollowState unitFollowState;
    UnitAttackState unitAttackState;
    Unit unit;

    public Transform targetToAttack;

    public bool isPlayer;

    //public bool isUnit;
    //UnitAttackState unitAttackState; 
    //UnitFollowState unitFollowState;
    //Animator animator;

    //private void Awake()
    //{
    //    animator = GetComponent<Animator>();

    //    // Получаем StateMachineBehaviour только если аниматор существует
    //    if (animator != null && animator.runtimeAnimatorController != null)
    //    {
    //        unitAttackState = animator.GetBehaviour<UnitAttackState>();
    //        unitFollowState = animator.GetBehaviour<UnitFollowState>();
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        animator = GetComponentInParent<Animator>();
        unit = GetComponentInParent<Unit>();
        unitAttackState = animator.GetBehaviour<UnitAttackState>();
        //unitFollowState = animator.GetBehaviour<UnitFollowState>();
        //if (isPlayer && other.CompareTag("Enemy"))
        //{
        //    //Debug.Log("Enemy to target!!");
        //    targetToAttack = other.transform;
        //}
        if (isPlayer && other.CompareTag("Enemy") && !animator.transform.CompareTag("Unit Monk Doctor") && targetToAttack == null)
        {
            Debug.Log("Enemy to target!!");
            targetToAttack = other.transform;
        }
        if (!isPlayer && (other.CompareTag("Friendly") || other.CompareTag("Unit Monk Doctor")) 
            && targetToAttack == null)
        {
            Debug.Log("Unit to target!!");
            targetToAttack = other.transform;
            //Debug.Log(other.transform.tag);
        }

        //if (other.CompareTag("Friendly") && targetToAttack == null)
        //{
        //    //Debug.Log("Friendly to target!!");
        //    targetToAttack = other.transform;
        //}

        //if (other.CompareTag("Building") && targetToAttack == null)
        //{
        //    //Debug.Log("Building to target!!");
        //    targetToAttack = other.transform;
        //}

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack != null)
        {
            targetToAttack = null;
        }

        if ((other.CompareTag("Friendly") || other.CompareTag("Unit Monk Doctor")) && targetToAttack != null)
        {
            targetToAttack = null;
        }

        if (other.CompareTag("Building") && targetToAttack != null)
        {
            targetToAttack = null;
        }

    }

    private void OnDrawGizmos()
    {
        // Follow Distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f); // Радиус сферического колладера на юните 

        //// Heal Distance
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, unitFollowState.attackingDistance); // Следует из UnitFollowState

        //// Stop Distance
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, unitAttackState.stopAttackingDistance); // Следует из UnitAttackstate

        // Heal Distance
        Gizmos.color = Color.red;
        if (unit != null)
        {
            Gizmos.DrawWireSphere(transform.position, unit.attackRange);
        }// Следует из UnitFollowState

        // Stop Distance
        Gizmos.color = Color.blue;
        if (unitAttackState != null)
        { 
            Gizmos.DrawWireSphere(transform.position, unitAttackState.stopAttackingDistance); // Следует из UnitAttackstate, Unit
        }
    }
}
