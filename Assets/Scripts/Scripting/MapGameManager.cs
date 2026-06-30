using UnityEngine;
using TMPro;

public class MapGameManager : MonoBehaviour
{
    [Header("Enemy Count For Logic")]
    public int aliveEnemyCount = 0;

    [Header("Wave Count For UI")]
    public int currentWaveNumber = 0;
    public int totalWaves = 3;
    public int currentWaveRemaining = 0;
    public int currentWaveTotal = 0;

    [Header("Boss")]
    public bool bossSpawned = false;
    public bool bossDead = false;

    [Header("UI")]
    public GameObject winPanel;

    [Header("Enemy UI")]
    public TMP_Text enemyCountText;

    [Header("Gold")]
    public int gold = 100;
    public TMP_Text goldText;

    private bool gameEnded = false;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        UpdateEnemyCountUI();
        UpdateGoldUI();
    }

    public void StartWave(int waveNumber, int totalWaveCount, int enemyCountInWave)
    {
        currentWaveNumber = waveNumber;
        totalWaves = totalWaveCount;

        currentWaveTotal = enemyCountInWave;
        currentWaveRemaining = enemyCountInWave;

        Debug.Log("Wave " + currentWaveNumber + " started with " + currentWaveRemaining + " enemies.");

        UpdateEnemyCountUI();
    }

    private void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            if (bossSpawned && !bossDead)
            {
                enemyCountText.text = "Boss: Skeleton King";
            }
            else if (currentWaveNumber > 0)
            {
                enemyCountText.text =
                    "Wave " + currentWaveNumber + "/" + totalWaves +
                    " - Enemies: " + currentWaveRemaining + "/" + currentWaveTotal;
            }
            else
            {
                enemyCountText.text = "Enemies: 0";
            }
        }
    }

    public void RegisterEnemySpawned()
    {
        aliveEnemyCount++;

        Debug.Log("Enemy spawned. Alive enemies: " + aliveEnemyCount);

        // Không update currentWaveRemaining ở đây.
        // Vì UI cần đếm ngược theo tổng quái của wave, không phải số quái đã spawn.
        UpdateEnemyCountUI();
    }

    public void RegisterEnemyRemoved()
    {
        aliveEnemyCount--;

        if (aliveEnemyCount < 0)
        {
            aliveEnemyCount = 0;
        }

        currentWaveRemaining--;

        if (currentWaveRemaining < 0)
        {
            currentWaveRemaining = 0;
        }

        Debug.Log("Enemy removed from wave. Remaining: " + currentWaveRemaining + "/" + currentWaveTotal);

        UpdateEnemyCountUI();
        CheckWinCondition();
    }

    public void RegisterBossSpawned()
    {
        bossSpawned = true;
        bossDead = false;

        Debug.Log("Boss spawned!");

        UpdateEnemyCountUI();
    }

    public void RegisterBossDied()
    {
        bossDead = true;

        Debug.Log("Boss defeated!");

        UpdateEnemyCountUI();
        CheckWinCondition();
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount)
        {
            Debug.Log("Not enough gold!");
            return false;
        }

        gold -= amount;
        UpdateGoldUI();
        return true;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold;
        }
    }

    private void CheckWinCondition()
    {
        if (gameEnded)
        {
            return;
        }

        if (bossSpawned && bossDead && aliveEnemyCount <= 0)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        gameEnded = true;

        Debug.Log("YOU WIN!");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}