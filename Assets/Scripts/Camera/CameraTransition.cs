using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public Transform targetPosition;
    public float moveSpeed = 2f;

    private bool shouldMove = false;

    void Update()
    {
        if (shouldMove)
        {
            Debug.Log("Camera moving to target posotion");
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition.position,
                moveSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetPosition.rotation,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition.position) <= 0f)
            {
                shouldMove = false;
            }
        }
    }

    public void StartCameraMovement()
    {
        shouldMove = true;
    }
}