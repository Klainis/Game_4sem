using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    public bool isUnit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }

        if (other.CompareTag("Friendly") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack != null)
        {
            targetToAttack = null;
        }

    }
}
