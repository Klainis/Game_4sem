using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuildingOption
{
    public GameObject prefab;
    public int cost;
}

public class BuildingUI : MonoBehaviour
{
    public BuildingPlacementManager placementManager;
    public BuildingOption[] buildingOptions;
    public GameObject floatingTextPrefab;

    public void SelectBuilding(int index)
    {
        if (placementManager != null &&
            index >= 0 && index < buildingOptions.Length)
        {
            placementManager.BeginPlacement(buildingOptions[index].prefab, buildingOptions[index].cost);
        }
    }

        public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
