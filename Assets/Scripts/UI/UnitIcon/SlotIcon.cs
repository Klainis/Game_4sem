//using UnityEditor.Build.Content;
//using UnityEngine;
//using UnityEngine.UI;


//public class SlotIcon : MonoBehaviour
//{
//    public Image iconPictures;
//    public Unit settings;
//    InterfaceScript interfaceScript;
//    UnitSelectionManager unitSelectionManager;
//    public int unitIndex;

//    private void Start()
//    {
//        unitSelectionManager = FindObjectOfType<UnitSelectionManager>();
//        interfaceScript = FindObjectOfType<InterfaceScript>();
//        iconPictures.sprite = settings.unitIconSprite;
//    } 

//    public void UpdateIndex(int index)
//    {
//        unitIndex = index;
//    }

//    public void ResetToSelect()
//    {
//        unitSelectionManager.unitSelected.Remove(unitSelectionManager.unitSelected[unitIndex]);
//        interfaceScript.iconsUI.Remove(interfaceScript.iconsUI[unitIndex]);
//        Destroy(gameObject);
//    }
//}
