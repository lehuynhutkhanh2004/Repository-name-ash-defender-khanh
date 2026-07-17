using System.Collections;
using UnityEngine;

public class NKSimpleWaveSpawner : MonoBehaviour
{
    [Header("Assign your enemy prefabs here")]
    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    [Header("Paths")]
    public NKPathContainer pathA;
    public NKPathContainer pathB;
    public NKPathContainer bossPath;

    [Header("Containers")]
    public Transform enemyContainer;
    public Transform bossContainer;

    [Header("Wave Settings")]
    public int wave1Count = 6;
    public int wave2Count = 8;
    public float spawnDelay = 1f;
    public bool autoStart = false;

    private Coroutine runningWave;

    private void Start()
    {
        if (autoStart)
        {
            StartWaves();
        }
    }

    public void StartWaves()
    {
        if (runningWave != null)
        {
            StopCoroutine(runningWave);
        }
        runningWave = StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        yield return SpawnEnemies(pathA, wave1Count, enemyPrefab, enemyContainer);
        yield return new WaitForSeconds(2f);
        yield return SpawnEnemies(pathB, wave2Count, enemyPrefab, enemyContainer);
        yield return new WaitForSeconds(2f);
        SpawnBoss();
    }

    private IEnumerator SpawnEnemies(NKPathContainer path, int count, GameObject prefab, Transform container)
    {
        if (prefab == null || path == null)
        {
            Debug.LogWarning("NKSimpleWaveSpawner missing enemy prefab or path.");
            yield break;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject enemy = Instantiate(prefab, container == null ? null : container);
            NKEnemyWaypointMover mover = enemy.GetComponent<NKEnemyWaypointMover>();
            if (mover == null)
            {
                mover = enemy.AddComponent<NKEnemyWaypointMover>();
            }
            mover.SetPath(path);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null || bossPath == null)
        {
            Debug.LogWarning("Boss prefab or boss path is missing.");
            return;
        }

        GameObject boss = Instantiate(bossPrefab, bossContainer == null ? null : bossContainer);
        NKEnemyWaypointMover mover = boss.GetComponent<NKEnemyWaypointMover>();
        if (mover == null)
        {
            mover = boss.AddComponent<NKEnemyWaypointMover>();
        }
        mover.moveSpeed = 1.2f;
        mover.SetPath(bossPath);
    }
}
