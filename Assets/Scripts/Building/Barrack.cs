using UnityEngine;

[AddComponentMenu("Building/Barrack")]
public class Barrack : ProductionBuilding
{
    [Header("Barracks Settings")]
    [SerializeField] private float spawnOffset = 2f; // Расстояние появления юнита от барака
    
    private void Reset()
    {
        maxHealth = 150; // Базовое здоровье для барака
        
        // Если точка спавна не установлена, создаём её перед бараком
        if (!spawnPoint)
        {
            GameObject spawnMarker = new GameObject("SpawnPoint");
            spawnMarker.transform.parent = transform;
            spawnMarker.transform.localPosition = Vector3.forward * spawnOffset;
            spawnPoint = spawnMarker.transform;
        }
    }

    // Переопределяем метод производства для добавления специфичной логики барака
    //public new void Produce(int index)
    //{
    //    if (index < 0 || index >= units.Length) return;
    //    var option = units[index];

    //    if (!ResourceManager.Instance.SpendGold(option.cost)) 
    //    {
    //        // Можно добавить визуальное или звуковое оповещение о нехватке золота
    //        Debug.Log("Недостаточно золота для найма юнита!");
    //        return;
    //    }

    //    Vector3 pos = (spawnPoint ? spawnPoint.position : transform.position + transform.forward * spawnOffset);
    //    GameObject unit = Instantiate(option.prefab, pos, Quaternion.identity);
    //    Debug.Log("ЮНИТ СОЗДАН");
        
    //    // Здесь можно добавить дополнительную инициализацию юнита
    //    // Например, установить команду, добавить эффекты появления и т.д.
    //}
}
