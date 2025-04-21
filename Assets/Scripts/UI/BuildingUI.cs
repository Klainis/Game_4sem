using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public BuildingPlacementManager placementManager;
    public GameObject[] buildingPrefabs;

    public void SelectBuilding(int index)
    {
        if (placementManager != null &&
            index >= 0 && index < buildingPrefabs.Length)
        {
            placementManager.BeginPlacement(buildingPrefabs[index]);
        }
    }
}
