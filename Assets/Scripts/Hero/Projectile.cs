using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float moveSpeed = 8f;
    public float hitDistance = 0.1f;

    private EnemyHealth target;
    private int damage;
    private bool hasHit = false;

    public void Init(EnemyHealth newTarget, int newDamage)
    {
        target = newTarget;
        damage = newDamage;
        hasHit = false;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (hasHit)
        {
            return;
        }

        Vector3 targetPosition = target.transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= hitDistance)
        {
            hasHit = true;

            target.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}