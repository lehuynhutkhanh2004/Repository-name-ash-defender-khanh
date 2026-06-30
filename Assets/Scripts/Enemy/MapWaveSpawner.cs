using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class WaveEnemyGroup
{
    public string enemyName;
    public EnemyPathFollower enemyPrefab;
    public int count = 1;
    public float spawnInterval = 0.7f;
}

[System.Serializable]
public class WaveDefinition
{
    public string waveName;
    public List<WaveEnemyGroup> enemyGroups = new List<WaveEnemyGroup>();
}

public class MapWaveSpawner : MonoBehaviour
{
    [Header("Waves")]
    public List<WaveDefinition> waves = new List<WaveDefinition>();
    public float timeBetweenWaves = 3f;
    public bool waitUntilWaveCleared = true;

    [Header("Spawn Points")]
    public Transform spawnPointA_Left;
    public Transform spawnPointB_Top;

    [Header("Paths")]
    public List<Transform> pathA_LeftToCore = new List<Transform>();
    public List<Transform> pathB_TopToCore = new List<Transform>();

    [Header("Containers")]
    public Transform enemyContainer;

    [Header("Boss")]
    public EnemyPathFollower bossPrefab;
    public Transform bossSpawnPoint;
    public Transform bossContainer;
    public List<Transform> bossPath = new List<Transform>();
    public float bossSpawnDelay = 3f;

    [Header("UI Optional")]
    public TMP_Text waveText;

    private int currentWaveIndex = 0;
    private int pathToggle = 0;
    private MapGameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<MapGameManager>();
        StartCoroutine(SpawnAllWavesThenBoss());
    }

    private IEnumerator SpawnAllWavesThenBoss()
    {
        yield return new WaitForSeconds(1f);

        for (currentWaveIndex = 0; currentWaveIndex < waves.Count; currentWaveIndex++)
        {
            WaveDefinition wave = waves[currentWaveIndex];

            int enemyCountInWave = GetWaveEnemyCount(wave);

            if (gameManager != null)
            {
                gameManager.StartWave(
                    currentWaveIndex + 1,
                    waves.Count,
                    enemyCountInWave
                );
            }

            UpdateWaveText("Wave " + (currentWaveIndex + 1) + "/" + waves.Count);
            Debug.Log("Starting " + wave.waveName);

            yield return StartCoroutine(SpawnWave(wave));

            if (waitUntilWaveCleared)
            {
                yield return new WaitUntil(() => gameManager == null || gameManager.aliveEnemyCount <= 0);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        UpdateWaveText("Boss incoming...");
        Debug.Log("All normal waves finished! Boss incoming!");

        yield return new WaitForSeconds(bossSpawnDelay);

        SpawnBoss();
    }

    private IEnumerator SpawnWave(WaveDefinition wave)
    {
        foreach (WaveEnemyGroup group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnOneEnemy(group.enemyPrefab);
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

    private int GetWaveEnemyCount(WaveDefinition wave)
    {
        int total = 0;

        foreach (WaveEnemyGroup group in wave.enemyGroups)
        {
            total += group.count;
        }

        return total;
    }
    private void SpawnOneEnemy(EnemyPathFollower enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is missing in wave!");
            return;
        }

        Transform selectedSpawnPoint;
        List<Transform> selectedPath;

        if (pathToggle % 2 == 0)
        {
            selectedSpawnPoint = spawnPointA_Left;
            selectedPath = pathA_LeftToCore;
        }
        else
        {
            selectedSpawnPoint = spawnPointB_Top;
            selectedPath = pathB_TopToCore;
        }

        pathToggle++;

        if (selectedSpawnPoint == null)
        {
            Debug.LogError("Spawn point is missing!");
            return;
        }

        EnemyPathFollower enemy = Instantiate(
            enemyPrefab,
            selectedSpawnPoint.position,
            Quaternion.identity
        );

        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.MarkAsWaveEnemy();
        }

        if (enemyContainer != null)
        {
            enemy.transform.SetParent(enemyContainer);
        }

        enemy.SetPath(selectedPath);

        if (gameManager != null)
        {
            gameManager.RegisterEnemySpawned();
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogError("Boss Prefab is missing!");
            return;
        }

        if (bossSpawnPoint == null)
        {
            Debug.LogError("Boss Spawn Point is missing!");
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

        boss.SetPath(bossPath);

        if (gameManager != null)
        {
            gameManager.RegisterBossSpawned();
        }

        UpdateWaveText("BOSS: Slime King");
        Debug.Log("Slime King spawned!");
    }

    private void UpdateWaveText(string text)
    {
        if (waveText != null)
        {
            waveText.text = text;
        }
    }
}