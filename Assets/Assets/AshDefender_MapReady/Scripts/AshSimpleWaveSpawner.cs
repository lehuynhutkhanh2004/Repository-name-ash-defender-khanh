using System.Collections;
using UnityEngine;

/// <summary>
/// Optional test spawner. Use it only if your current project does not already have a wave system.
/// Assign an enemy prefab and a path, then press Play.
/// </summary>
public class AshSimpleWaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private AshPathContainer path;
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float enemyMoveSpeed = 2.5f;

    private IEnumerator Start()
    {
        if (enemyPrefab == null || path == null)
        {
            Debug.Log("AshSimpleWaveSpawner is ready, but Enemy Prefab or Path is not assigned yet.");
            yield break;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            AshEnemyWaypointMover mover = enemy.GetComponent<AshEnemyWaypointMover>();
            if (mover == null)
            {
                mover = enemy.AddComponent<AshEnemyWaypointMover>();
            }

            mover.Setup(path, enemyMoveSpeed);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
