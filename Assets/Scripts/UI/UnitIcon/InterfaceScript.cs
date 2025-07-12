using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceScript : MonoBehaviour
{
    public GameObject unitIconPref;
    public Transform CharUIListTransform;

    public List<GameObject> iconsUI = new List<GameObject>();

    public void GetIconUnitToList(Unit unit, int index)
    {
        GameObject icon = Instantiate(unitIconPref, transform.position, Quaternion.identity);
        icon.transform.parent = CharUIListTransform;
        //icon.GetComponent<SlotIcon>().settings = 
        icon.GetComponent<SlotIcon>().unitIndex = index;
        unit.slot = icon.GetComponent<SlotIcon>();
        iconsUI.Add(icon);
    }

    public void IconsClear()
    {
        foreach(GameObject iconUI in iconsUI)
        {
            Destroy(iconUI);
        }
        iconsUI.Clear();
    }
}
