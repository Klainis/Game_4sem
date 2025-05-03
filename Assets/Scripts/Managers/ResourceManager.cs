using System;
using UnityEngine;

/// <summary>
/// Единственный источник правды о количестве золота.
/// Не знает ничего про UI, бросает событие при изменении.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] int startGold = 500;
    public  int  Gold { get; private set; }

    public event Action<int> OnGoldChanged;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Gold     = startGold;
    }

    public bool SpendGold(int amount)
    {
        if (Gold < amount) { Debug.Log("Недостаточно золота!"); return false; }

        Gold -= amount;
        OnGoldChanged?.Invoke(Gold);
        return true;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }
}
