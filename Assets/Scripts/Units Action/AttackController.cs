using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")  && targetToAttack == null)
        {
            //Debug.Log("Enemy to target!!");
            targetToAttack = other.transform;
        }

        if (other.CompareTag("Friendly") && targetToAttack == null)
        {
            //Debug.Log("Friendly to target!!");
            targetToAttack = other.transform;
        }

        if (other.CompareTag("Building") && targetToAttack == null)
        {
            //Debug.Log("Building to target!!");
            targetToAttack = other.transform;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Enemy") && targetToAttack != null)
        //{
        //    targetToAttack = null;
        //}

        //if (other.CompareTag("Friendly") && targetToAttack != null)
        //{
        //    targetToAttack = null;
        //}

        //if (other.CompareTag("Building") && targetToAttack != null)
        //{
        //    targetToAttack = null;
        //}

    }

    private void OnDrawGizmos()
    {
        // Follow Distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f); // Радиус сферического колладера на юните 

        //// Attack Distance
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, unitFollowState.attackingDistance); // Следует из UnitFollowState

        //// Stop Distance
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, unitAttackState.stopAttackingDistance); // Следует из UnitAttackstate

        // Attack Distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f); // Следует из UnitFollowState

        // Stop Distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 5.2f); // Следует из UnitAttackstate
    }
}
