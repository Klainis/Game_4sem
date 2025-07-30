using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System;

public class MinimapClickHandler : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform minimapRectTransform;

    public RTSCameraController cameraController;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
            return;

        Vector2 normalized = RectPointToNormalized(localPoint, minimapRectTransform);
        Ray ray = minimapCamera.ViewportPointToRay(new Vector3(normalized.x, normalized.y, 0));

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            MoveCameraToPoint(hit.point, hit);
        }

        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            MoveUnitsToPoint(hit.point);
        }
    }

    private void MoveCameraToPoint(Vector3 hitPoint, RaycastHit hit)
    {
        Vector3 offset = mainCamera.transform.position - mainCamera.transform.parent.position;
        Vector3 targetParentPos = hitPoint - offset;

        float distanceToPlane = hit.distance;
        targetParentPos.x += (float)(distanceToPlane * Math.Sin(mainCamera.transform.rotation.x));

        targetParentPos.x = Mathf.Clamp(targetParentPos.x, cameraController.minX, cameraController.maxX);
        targetParentPos.z = Mathf.Clamp(targetParentPos.z, cameraController.minZ, cameraController.maxZ);

        Transform camParent = mainCamera.transform.parent;
        camParent.position = new Vector3(targetParentPos.x, camParent.position.y, targetParentPos.z);

        cameraController.SetNewPosition(camParent.position);
    }

    private void MoveUnitsToPoint(Vector3 destination)
    {
        var selectedUnits = UnitSelectionManager.Instance.unitSelected;

        foreach (GameObject unit in selectedUnits)
        {
            if (unit.TryGetComponent(out NavMeshAgent agent) &&
                unit.TryGetComponent(out UnitMovement movement) &&
                unit.TryGetComponent(out Animator animator))
            {
                movement.isCommandedToMove = true;
                movement.isFollowingTarget = false;
                movement.lastMoveCommandTime = Time.time;

                animator.SetBool("isMoving", true);
                agent.SetDestination(destination);

                Vector3 dir = (destination - unit.transform.position).normalized;
                if (dir != Vector3.zero)
                    unit.transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }

    private Vector2 RectPointToNormalized(Vector2 localPoint, RectTransform rect)
    {
        Rect r = rect.rect;
        float x = (localPoint.x - r.x) / r.width;
        float y = (localPoint.y - r.y) / r.height;
        return new Vector2(x, y);
    }
}
