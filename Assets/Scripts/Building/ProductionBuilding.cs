using UnityEngine;

/// <summary>
/// Базовый класс для зданий, производящих юнитов.
/// </summary>
public abstract class ProductionBuilding : MonoBehaviour
{
    [System.Serializable]
    public struct UnitOption
    {
        public string     name;
        public GameObject prefab;
        public int        cost;
    }

    [Header("Список юнитов")]
    public UnitOption[] units;

    [Header("Точка появления")]
    public Transform spawnPoint;

    /*–––– защита от раннего клика после установки ––––*/
    bool interactive = false;

    public void EnableInteraction() => interactive = true;  // вызовет менеджер построек

    /*–––– API для UnitProductionPanel ––––*/
    public void Produce(int index)
    {
        if (index < 0 || index >= units.Length) return;
        var option = units[index];

        if (!ResourceManager.Instance.SpendGold(option.cost)) return;

        Vector3 pos = (spawnPoint ? spawnPoint.position
                                  : transform.position + transform.forward * 2);
        Instantiate(option.prefab, pos, Quaternion.identity);
    }

    /*–––– обработка клика ––––*/
    void OnMouseDown()
    {
        if (!interactive) return;                                  // ← блокируем «только что построенное»
        UnitProductionPanel.Instance.Toggle(this);                 // ← переключатель
    }
}
