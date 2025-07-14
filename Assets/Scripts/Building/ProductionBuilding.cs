using UnityEngine;
using System.Collections;

/// <summary>
/// Базовый класс для зданий, производящих юнитов.
/// </summary>
public abstract class ProductionBuilding : BuildingBase
{
    [System.Serializable]
    public struct UnitOption
    {
        public string name;
        public GameObject prefab;
        public int cost;
    }

    [Header("Список юнитов")]
    public UnitOption[] units;

    [Header("Точка появления")]
    public Transform spawnPoint;

    [Header("UI")]
    [SerializeField] ProgressBarUI progressBar;

    private bool isProducing = false;

    private void Start()
    {
        if(progressBar == null) progressBar = GetComponentInChildren<ProgressBarUI>();
        progressBar?.Hide();
    }

    /*–––– защита от раннего клика после установки ––––*/
    bool interactive = true;

    public void EnableInteraction() => interactive = true;  // вызовет менеджер построек

    /*–––– API для UnitProductionPanel ––––*/
    public void Produce(int index)
    {
        if (index < 0 || index >= units.Length) return;
        if (isProducing)
        {
            Debug.Log("Здание уже производит юнита!");
            return;
        }

        var option = units[index];

        if (!ResourceManager.Instance.SpendGold(option.cost)) return;

        StartCoroutine(ProductionRoutine(option));
    }

    private IEnumerator ProductionRoutine(UnitOption option)
    {
        isProducing = true;
        float timer = 0;
        float productionTime = 0.3f; // Уменьшили время найма до 2 секунд

        while(timer < productionTime)
        {
            timer += Time.deltaTime;
            progressBar?.UpdateProgress(timer / productionTime, $"Найм: {option.name}");
            // Убеждаемся, что ProgressBar ориентирован правильно
            progressBar?.ForceCorrectOrientation();
            yield return null;
        }

        Vector3 pos = (spawnPoint ? spawnPoint.position
                                  : transform.position + transform.forward * 2);
        Instantiate(option.prefab, pos, Quaternion.identity);
        Debug.Log("ЮНИТ СОЗДАН");

        isProducing = false;
        progressBar?.Hide();
    }

    /*–––– обработка клика ––––*/
    void OnMouseDown()
    {
        if (!interactive) return;                                  // ← блокируем «только что построенное»
        UnitProductionPanel.Instance.Toggle(this);                 // ← переключатель
    }
    
    private void Reset() { maxHealth = 120; } // базовое здоровье бараков
}
