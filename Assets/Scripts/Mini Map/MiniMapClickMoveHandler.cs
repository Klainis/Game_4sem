using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class MinimapClickMoveHandler : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Camera minimapCamera; // Камера миникарты
    [SerializeField] private Camera mainCamera;    // Основная RTS-камера
    [SerializeField] private RectTransform minimapRectTransform; // RawImage RectTransform

    NavMeshAgent agent;
    UnitMovement unitMovement;
    Animator animator;

    public void OnPointerDown(PointerEventData eventData)
    {
       if (eventData.button == PointerEventData.InputButton.Right/* && !EventSystem.current.IsPointerOverGameObject()*/)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
                return;

            Vector2 normalized = RectPointToNormalized(localPoint, minimapRectTransform);
            //Обрабатываем точку на карте
            Ray ray = minimapCamera.ViewportPointToRay(new Vector3(normalized.x, normalized.y, 0));
            if (Physics.Raycast(ray, out RaycastHit hitMap))
            {
                UnitSelectionManager unitSelManager = UnitSelectionManager.Instance;
                List<GameObject> unitSel = unitSelManager.unitSelected;

                if (unitSel.Count > 0)
                {
                    foreach (var unit in unitSel)
                    {
                        agent = unit.GetComponent<NavMeshAgent>();
                        unitMovement = unit.GetComponent<UnitMovement>();
                        animator = unit.GetComponent<Animator>();

                        unitMovement.isCommandedToMove = true;
                        unitMovement.isFollowingTarget = false;

                        animator.SetBool("isMoving", true);
                        unitMovement.lastMoveCommandTime = Time.time;

                        // Поворачиваем юнита в сторону движения
                        Vector3 direction = (hitMap.point - transform.position).normalized;
                        if (direction != Vector3.zero)
                        {
                            unit.transform.rotation = Quaternion.LookRotation(direction);
                        }

                        agent.SetDestination(hitMap.point);
                    }
                }
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
