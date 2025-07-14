using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private Vector3 finalScale = Vector3.one;
    
    private Vector3 startScale;
    private float timer;
    
    void Start()
    {
        startScale = Vector3.zero;
        transform.localScale = startScale;
        
        // Автоматически уничтожаем через duration + небольшой запас
        Destroy(gameObject, duration + 0.5f);
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;
        
        if (progress <= 1f)
        {
            // Анимация масштаба
            float scaleValue = scaleCurve.Evaluate(progress);
            transform.localScale = Vector3.Lerp(startScale, finalScale, scaleValue);
            
            // Вращение
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Завершаем анимацию
            transform.localScale = finalScale;
        }
    }
} 