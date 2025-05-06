using System.Collections;
using UnityEngine;

/// <summary>
/// Здание-добытчик: ищет ближайший ResourceNode и
/// каждые N секунд извлекает золото.
/// </summary>
public class GoldMine : MonoBehaviour
{
    [Header("Search")]
    [SerializeField] float searchRadius = 3f;

    [Header("Mining")]
    [SerializeField] float tickSeconds = 2f;
    [SerializeField] int   goldPerTick = 10;

    ResourceNode node;
    Coroutine    routine;

    void Start()
    {
        node = FindClosestNode();
        if (node != null)
        {
            routine = StartCoroutine(MineLoop());
            node.OnDepleted += HandleNodeDepleted;
        }
        else
        {
            Debug.LogWarning($"{name}: рядом нет Gold Deposit — шахта бездействует");
        }
    }

    ResourceNode FindClosestNode()
    {
        var colliders = Physics.OverlapSphere(transform.position, searchRadius);
        float bestDist = float.MaxValue;
        ResourceNode bestNode = null;

        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out ResourceNode rn) && !rn.IsDepleted)
            {
                float d = (col.transform.position - transform.position).sqrMagnitude;
                if (d < bestDist) { bestDist = d; bestNode = rn; }
            }
        }
        return bestNode;
    }

    IEnumerator MineLoop()
    {
        while (node != null && !node.IsDepleted)
        {
            yield return new WaitForSeconds(tickSeconds);

            int taken = node.Extract(goldPerTick);
            if (taken > 0)
                ResourceManager.Instance.AddGold(taken);   // :contentReference[oaicite:0]{index=0}:contentReference[oaicite:1]{index=1}
        }
    }

    void HandleNodeDepleted(ResourceNode _)
    {
        if (routine != null) StopCoroutine(routine);
        Debug.Log($"{name}: ресурс исчерпан, шахта остановилась");
    }
}
