using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapClickHandler : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Camera minimapCamera; // Камера миникарты
    [SerializeField] private Camera mainCamera;    // Основная RTS-камера
    [SerializeField] private RectTransform minimapRectTransform; // RawImage RectTransform

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
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
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                float distanceToPlane = hit.distance;
                Vector3 hitPoint = hit.point;

                RTSCameraController controller = RTSCameraController.instance;

                Vector3 cameraOffset = mainCamera.transform.position - mainCamera.transform.parent.position;

                Vector3 targetParentPos = hitPoint - cameraOffset;

                targetParentPos.x += (float)(distanceToPlane * Math.Sin(mainCamera.transform.rotation.x));

                targetParentPos.x = Mathf.Clamp(targetParentPos.x, controller.minX, controller.maxX);
                targetParentPos.z = Mathf.Clamp(targetParentPos.z, controller.minZ, controller.maxZ);

                // Применяем позицию
                Transform camParent = mainCamera.transform.parent;
                camParent.position = new Vector3(targetParentPos.x, camParent.position.y, targetParentPos.z);

                controller.SetNewPosition(camParent.position);//Координаты для CameraController
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
