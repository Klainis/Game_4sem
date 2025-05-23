using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 15f;
    [SerializeField] float maxLifetime = 5f;
    [SerializeField] GameObject hitEffect;  // опциональный эффект при попадании

    Transform target;
    int damage;
    float lifetime;

    public void Init(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    void Update()
    {
        if (!target)
        {
            Destroy(gameObject);
            return;
        }

        // Движение к цели
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        // Проверка попадания
        float distanceThisFrame = speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, target.position) <= distanceThisFrame)
        {
            Hit();
        }

        // Уничтожение по таймеру
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    void Hit()
    {
        if (target.TryGetComponent(out Unit unit))
        {
            unit.TakeDamage(damage);
        }

        if (hitEffect)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}