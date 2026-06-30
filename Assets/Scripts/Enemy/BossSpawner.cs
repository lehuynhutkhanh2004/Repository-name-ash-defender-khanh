using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public EnemyPathFollower bossPrefab;
    public Transform bossSpawnPoint;
    public Transform bossContainer;
    public List<Transform> bossWaypoints = new List<Transform>();

    public void SpawnBoss()
    {
        if (bossPrefab == null || bossSpawnPoint == null)
        {
            Debug.LogError("Boss prefab or spawn point missing!");
            return;
        }

        EnemyPathFollower boss = Instantiate(
            bossPrefab,
            bossSpawnPoint.position,
            Quaternion.identity
        );

        if (bossContainer != null)
        {
            boss.transform.SetParent(bossContainer);
        }

        boss.SetPath(bossWaypoints);

        MapGameManager gameManager = FindFirstObjectByType<MapGameManager>();

        if (gameManager != null)
        {
            gameManager.RegisterBossSpawned();
        }
    }
}