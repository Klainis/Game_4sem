using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    //public bool isUnit;

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
        if (other.CompareTag("Enemy") && targetToAttack != null)
        {
            targetToAttack = null;
        }

        if (other.CompareTag("Friendly") && targetToAttack != null)
        {
            targetToAttack = null;
        }

        if (other.CompareTag("Building") && targetToAttack != null)
        {
            targetToAttack = null;
        }

    }
}
