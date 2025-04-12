using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    void Start()
    {
        UnitSelectionManager.Instance.allUnitList.Add(gameObject);
    }

    private void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitList.Remove(gameObject);
    }
}
