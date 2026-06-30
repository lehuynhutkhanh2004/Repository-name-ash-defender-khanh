using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public EnemyPathFollower enemyPrefab;
    public Transform enemyContainer;

    [Header("Spawn")]
    public Transform spawnPoint;
    public int spawnCount = 5;
    public float spawnInterval = 1f;

    [Header("Path")]
    public List<Transform> waypoints = new List<Transform>();

    [Header("Auto Start")]
    public bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnOneEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOneEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is missing!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point is missing!");
            return;
        }

        EnemyPathFollower enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        MapGameManager gameManager = FindFirstObjectByType<MapGameManager>();

        if (gameManager != null)
        {
            gameManager.RegisterEnemySpawned();
        }

        if (enemyContainer != null)
        {
            enemy.transform.SetParent(enemyContainer);
        }

        enemy.SetPath(waypoints);
    }
}