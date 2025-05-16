using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapController : MonoBehaviour
{
    public Camera miniMapCamera;
    public RawImage miniMapDisplay;
    public GameObject unitMarkerPrefab;
    public GameObject enemyMarkerPrefab;

    public Transform playerUnitsParent;
    public Transform enemyUnitsParent;// ������������ ������ ������ ������
    public string enemyTag = "Enemy";  // ��� ��������� ������

    private void Update()
    {
        UpdateUnitMarkers();
    }

    private void UpdateUnitMarkers()
    {
        // ������� ������ �������
        //foreach (Transform child in transform)
        //{
        //    Destroy(child.gameObject);
        //}

        UpdateFactionMarkers(playerUnitsParent);
        UpdateFactionMarkers(enemyUnitsParent);
        //ClearMarkers();
    }

    //private void ClearMarkers()
    //{
    //    List<GameObject> children = new List<GameObject>();

    //    for (int i = 0; i < parent.childCount; i++)
    //    {
    //        CreateMarker(parent.GetChild(i));
    //Destroy(child.gameObject)
    //    }
    //}

    private void UpdateFactionMarkers(Transform parent)
    {
        List<GameObject> children = new List<GameObject>();

        for (int i = 0; i < parent.childCount; i++)
        {
            CreateMarker(parent.GetChild(i));
        }
    }

    private void CreateMarker(Transform unit)
    {
        // ������� ������
        var marker = Instantiate(unitMarkerPrefab, transform);
        //marker.GetComponent<Image>().color = color;

        // ����������� ������� ������� � ������� �����
        Vector3 viewportPos = miniMapCamera.WorldToViewportPoint(unit.position);
        RectTransform mapRect = miniMapDisplay.rectTransform;

        marker.transform.localPosition = new Vector3(
            (viewportPos.x * mapRect.rect.width) - (mapRect.rect.width / 2),
            (viewportPos.y * mapRect.rect.height) - (mapRect.rect.height / 2),
            0
        );
    }
}