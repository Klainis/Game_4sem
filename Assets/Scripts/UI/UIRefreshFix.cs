using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIRefreshFix : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);

            foreach (var result in results)
            {
                GameObject go = result.gameObject;

                // Принудительно активируем PointerEnter + Down + Click
                ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerEnterHandler);
                ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerClickHandler);

                break; // только первый объект
            }
        }
    }
}
