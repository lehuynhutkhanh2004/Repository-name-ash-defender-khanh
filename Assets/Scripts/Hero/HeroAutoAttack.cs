using UnityEngine;

public class HeroAutoAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 5f;
    public float attackCooldown = 0.5f;
    public int damage = 10;

    [Header("Projectile")]
    public Projectile projectilePrefab;
    public Transform firePoint;
    public Transform projectileContainer;

    [Header("Visual")]
    public bool flipByTargetDirection = true;

    private float attackTimer = 0f;
    private EnemyHealth currentTarget;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        FindNearestEnemy();

        if (currentTarget != null)
        {
            FlipToTarget();

            if (attackTimer <= 0f)
            {
                Shoot();
                attackTimer = attackCooldown;
            }
        }
    }

    private void FindNearestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);

        EnemyHealth nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (EnemyHealth enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance <= attackRange && distance < nearestDistance)
            {
                nearestEnemy = enemy;
                nearestDistance = distance;
            }
        }

        currentTarget = nearestEnemy;
    }

    private void FlipToTarget()
    {
        if (!flipByTargetDirection || spriteRenderer == null || currentTarget == null)
        {
            return;
        }

        float directionX = currentTarget.transform.position.x - transform.position.x;

        if (directionX > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (directionX < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is missing on Hero!");
            return;
        }

        Vector3 spawnPosition = transform.position;

        if (firePoint != null)
        {
            spawnPosition = firePoint.position;
        }

        Projectile projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        if (projectileContainer != null)
        {
            projectile.transform.SetParent(projectileContainer);
        }

        projectile.Init(currentTarget, damage);

        Debug.Log("Hero shoots " + currentTarget.name);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}