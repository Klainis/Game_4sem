using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Шахта: каждые N секунд берёт goldPerTick из КАЖДОЙ жилы в радиусе.
/// </summary>
public class GoldMine : BuildingBase
{
    [Header("References")]
    [SerializeField] RangeVisualizer range;          // кольцо для игрока
    [SerializeField] ProgressBarUI progressBar;

    [Header("Mining")]
    [SerializeField] float tickSeconds = 2f;         // период добычи
    [SerializeField] int goldPerTick = 10;         // из каждой жилы

    readonly List<ResourceNode> nodes = new();       // все жилы в радиусе
    Coroutine routine;

    private void Reset() { maxHealth = 100; } // базовое здоровье шахты
    
    protected override void Awake()
    {
        base.Awake(); // Вызываем Awake базового класса
        if(progressBar == null) progressBar = GetComponentInChildren<ProgressBarUI>();
        
        if (!range)
            range = GetComponentInChildren<RangeVisualizer>(true);

        range?.Hide();                               // шахта стартует без видимого кольца
        progressBar?.Hide();

        float radius = range ? range.RadiusWorld : 3f;
        FindNodes(radius);

        if (nodes.Count > 0)
            routine = StartCoroutine(MineLoop());
        else
            Debug.Log($"{name}: рядом нет Gold Deposit");
    }

    void FindNodes(float radius)
    {
        var cols = Physics.OverlapSphere(transform.position, radius);
        foreach (var c in cols)
            if (c.TryGetComponent(out ResourceNode rn) && !rn.IsDepleted)
            {
                nodes.Add(rn);
                rn.OnDepleted += HandleNodeDepleted;
            }
    }

    IEnumerator MineLoop()
    {
        while (nodes.Count > 0)
        {
            float timer = 0;
            while(timer < tickSeconds)
            {
                timer += Time.deltaTime;
                progressBar?.UpdateProgress(timer / tickSeconds, $"Добыча...");
                // Убеждаемся, что ProgressBar ориентирован правильно
                progressBar?.ForceCorrectOrientation();
                yield return null;
            }

            int totalTaken = 0;
            // идём по копии, потому что в цикле список может измениться
            foreach (var node in new List<ResourceNode>(nodes))
            {
                int taken = node.Extract(goldPerTick);
                totalTaken += taken;
                if (taken == 0 && node.IsDepleted)
                    HandleNodeDepleted(node);
            }

            if (totalTaken > 0)
                ResourceManager.Instance.AddGold(totalTaken);
        }

        progressBar?.Hide();
        Debug.Log($"{name}: все жилы исчерпаны, добыча остановлена");
    }

    void HandleNodeDepleted(ResourceNode node)
    {
        node.OnDepleted -= HandleNodeDepleted;
        nodes.Remove(node);
        Destroy(node.transform.gameObject);
    }

    /* UI: показать / скрыть радиус */
    void OnMouseDown() { if (range) range.Toggle(); }
}
