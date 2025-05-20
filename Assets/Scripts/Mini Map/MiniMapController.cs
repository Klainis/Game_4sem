//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class MiniMapController : MonoBehaviour
//{
//    [Header("References")]
//    public GameObject unitMarkerPrefab;
//    public GameObject buildingMarkerPrefab;
//    public GameObject enemyMarkerPrefab;

//    [Header("Real World Parents")]
//    public Transform realUnitsParent;
//    public Transform realBuildingsParent;
//    public Transform realEnemiesParent;
//    public Transform realEnemyBuildingsParent;

//    [Header("MiniMap Parents")]
//    public Transform miniMapUnitsParent;
//    public Transform miniMapBuildingsParent;
//    public Transform miniMapEnemiesParent;

//    [Header("Pool Settings")]
//    public int unitsPoolSize = 20;
//    public int buildingsPoolSize = 10;
//    public int enemiesPoolSize = 20;

//    [Header("Display Settings")]
//    public Color playerUnitColor = Color.blue;
//    public Color buildingColor = Color.gray;
//    public Color enemyColor = Color.red;
//    private RectTransform mapWorld;
//    private Vector2 mapWorldSize;
//    public float updateInterval = 0.2f;

//    private float nextUpdateTime;
//    private Queue<GameObject> unitsPool = new Queue<GameObject>();
//    private Queue<GameObject> buildingsPool = new Queue<GameObject>();
//    private Queue<GameObject> enemiesPool = new Queue<GameObject>();

//    private void Awake()
//    {
//        mapWorld = GetComponent<RectTransform>();
//        //mapWorldSize = new Vector2((mapWorld.sizeDelta.x * mapWorld.localScale.x),
//        //    (mapWorld.sizeDelta.y * mapWorld.localScale.y));
//        mapWorldSize = mapWorld.rect.size;

//        InitializePool(unitsPool, unitMarkerPrefab, miniMapUnitsParent, unitsPoolSize);
//        InitializePool(buildingsPool, buildingMarkerPrefab, miniMapBuildingsParent, buildingsPoolSize);
//        InitializePool(enemiesPool, enemyMarkerPrefab, miniMapEnemiesParent, enemiesPoolSize);
//    }

//    private void InitializePool(Queue<GameObject> pool, GameObject prefab, Transform parent, int size)
//    {
//        if (prefab == null || parent == null) return;

//        for (int i = 0; i < size; i++)
//        {
//            GameObject obj = Instantiate(prefab, parent);
//            obj.SetActive(false);
//            pool.Enqueue(obj);
//        }
//    }

//    private void Update()
//    {
//        if (Time.time >= nextUpdateTime)
//        {
//            //UpdateMiniMapElements();
//            nextUpdateTime = Time.time + updateInterval;
//        }
//    }

//    private void UpdateMiniMapElements()
//    {
//        ReturnAllToPool(unitsPool, miniMapUnitsParent);
//        ReturnAllToPool(buildingsPool, miniMapBuildingsParent);
//        ReturnAllToPool(enemiesPool, miniMapEnemiesParent);

//        UpdateUnits();
//        UpdateBuildings();
//        UpdateEnemies();
//    }

//    private void UpdateUnits()
//    {
//        if (realUnitsParent == null) return;

//        foreach (Transform unit in realUnitsParent)
//        {
//            if (unit == null) continue;

//            GameObject element = GetPooledObject(unitsPool, unitMarkerPrefab, miniMapUnitsParent);
//            element.transform.localPosition = WorldToMiniMapPosition(unit.position);
//            element.GetComponent<Image>().color = playerUnitColor;
//        }
//    }

//    private void UpdateBuildings()
//    {
//        if (realBuildingsParent == null) return;

//        foreach (Transform building in realBuildingsParent)
//        {
//            if (building == null) continue;

//            GameObject element = GetPooledObject(buildingsPool, buildingMarkerPrefab, miniMapBuildingsParent);
//            element.transform.localPosition = WorldToMiniMapPosition(building.position);
//            element.GetComponent<Image>().color = buildingColor;
//        }
//    }

//    private void UpdateEnemies()
//    {
//        if (realEnemiesParent == null) return;

//        foreach (Transform enemy in realEnemiesParent)
//        {
//            if (enemy == null) continue;

//            GameObject element = GetPooledObject(enemiesPool, enemyMarkerPrefab, miniMapEnemiesParent);
//            element.transform.localPosition = WorldToMiniMapPosition(enemy.position);
//            element.GetComponent<Image>().color = enemyColor;
//        }

//        if (realEnemyBuildingsParent == null) return;

//        foreach (Transform enemyBuilding in realEnemyBuildingsParent)
//        {
//            if (enemyBuilding == null) continue;

//            GameObject element = GetPooledObject(enemiesPool, enemyMarkerPrefab, miniMapEnemiesParent);
//            element.transform.localPosition = WorldToMiniMapPosition(enemyBuilding.position);
//            element.GetComponent<Image>().color = enemyColor;
//        }
//    }

//    private GameObject GetPooledObject(Queue<GameObject> pool, GameObject prefab, Transform parent)
//    {
//        if (pool.Count > 0)
//        {
//            GameObject obj = pool.Dequeue();
//            obj.SetActive(true);
//            return obj;
//        }

//        if (parent.childCount < 100)
//        {
//            return Instantiate(prefab, parent);
//        }
//        return null;
//    }

//    private Vector3 WorldToMiniMapPosition(Vector3 worldPos)
//    {
//        float x = (worldPos.x / mapWorldSize.x) * 100f; // Умножаем на 100 для получения процентов
//        float z = (worldPos.z / mapWorldSize.y) * 100f;
//        return new Vector3(x, z, 0);
//    }

//    private void ReturnAllToPool(Queue<GameObject> pool, Transform parent)
//    {
//        int childCount = parent.childCount;
//        for (int i = childCount - 1; i >= 0; i--)
//        {
//            Transform child = parent.GetChild(i);
//            if (child.gameObject.activeSelf)
//            {
//                child.gameObject.SetActive(false);
//                pool.Enqueue(child.gameObject);
//            }
//        }
//    }
//}

using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public RectTransform mapRect;
    public Transform playerUnitsRoot;
    public Transform enemyUnitsRoot;
    public Transform buildingsRoot;

    public Transform unitMapMarker;
    public Transform enemyMapMarker;
    public Transform buildingsMapMarker;

    public GameObject unitMarkerPrefab;
    public GameObject enemyMarkerPrefab;
    public GameObject buildingMarkerPrefab;

    private Vector2 mapSize;
    private RectTransform map;
    private RectTransform markersParent;

    void Start()
    {
        map = GetComponent<RectTransform>();
        mapSize = map.rect.size;
        markersParent = mapRect;
        CreateMarkers(playerUnitsRoot, unitMarkerPrefab, unitMapMarker);
        CreateMarkers(enemyUnitsRoot, enemyMarkerPrefab, enemyMapMarker);
        CreateMarkers(buildingsRoot, buildingMarkerPrefab, buildingsMapMarker);
    }

    void LateUpdate()
    {
        UpdateMarkers(playerUnitsRoot, unitMapMarker);
        UpdateMarkers(enemyUnitsRoot, enemyMapMarker);
        UpdateMarkers(buildingsRoot, buildingsMapMarker);
    }

    void CreateMarkers(Transform parent, GameObject prefab, Transform mapMarker)
    {
        foreach (Transform t in parent)
            Instantiate(prefab, mapMarker).name = t.name;
    }

    void UpdateMarkers(Transform worldGroup, Transform mapMarker)
    {
        for (int i = 0; i < worldGroup.childCount; i++)
        {
            var worldPos = worldGroup.GetChild(i).position;
            var normalizedPos = new Vector2(worldPos.x / mapSize.x, worldPos.z / mapSize.y);
            var marker = mapMarker.GetComponent<RectTransform>();
            marker.anchoredPosition = new Vector2(
                normalizedPos.x * mapRect.rect.width,
                normalizedPos.y * mapRect.rect.height
            );
        }
    }
}
