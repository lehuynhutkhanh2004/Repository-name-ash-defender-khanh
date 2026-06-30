using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFollower : MonoBehaviour
{
    [Header("Path")]
    public List<Transform> waypoints = new List<Transform>();

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float reachDistance = 0.05f;

    [Header("Visual")]
    public bool flipSpriteByDirection = true;

    private int currentWaypointIndex = 0;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            return;
        }

        if (currentWaypointIndex >= waypoints.Count)
        {
            ReachCore();
            return;
        }

        MoveToCurrentWaypoint();
    }

    private void MoveToCurrentWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = targetWaypoint.position;

        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (flipSpriteByDirection && spriteRenderer != null)
        {
            float directionX = targetPosition.x - currentPosition.x;

            if (directionX > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (directionX < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= reachDistance)
        {
            currentWaypointIndex++;
        }
    }

    private void ReachCore()
    {
        EnemyHealth enemyHealth = GetComponent<EnemyHealth>();

        int damageToCore = 10;

        if (enemyHealth != null)
        {
            damageToCore = enemyHealth.coreDamage;
        }

        BaseCoreHealth core = FindFirstObjectByType<BaseCoreHealth>();

        if (core != null)
        {
            core.TakeDamage(damageToCore);
        }

        Debug.Log(gameObject.name + " reached the Core!");

        if (enemyHealth != null)
        {
            enemyHealth.RemoveBecauseReachedCore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPath(List<Transform> newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypointIndex = 0;
    }
}