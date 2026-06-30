using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    public int maxHP = 30;
    public int currentHP;

    [Header("Enemy Type")]
    public bool isBoss = false;

    [Header("Core Damage")]
    public int coreDamage = 10;

    [Header("Reward")]
    public int goldReward = 10;

    [Header("Wave Count")]
    public bool isWaveEnemy = false;

    private bool isDead = false;
    private bool hasReportedRemoved = false;

    private void Awake()
    {
        currentHP = maxHP;
        isDead = false;
        hasReportedRemoved = false;
    }

    public void MarkAsWaveEnemy()
    {
        isWaveEnemy = true;
        hasReportedRemoved = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHP -= damage;

        Debug.Log(gameObject.name + " took damage: " + damage);
        Debug.Log(gameObject.name + " HP: " + currentHP + "/" + maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log(gameObject.name + " died!");

        MapGameManager gameManager = FindFirstObjectByType<MapGameManager>();

        if (gameManager != null)
        {
            if (isBoss)
            {
                gameManager.AddGold(50);
                gameManager.RegisterBossDied();
            }
            else
            {
                gameManager.AddGold(goldReward);
                ReportRemovedFromWave(gameManager);
            }
        }

        Destroy(gameObject);
    }

    public void RemoveBecauseReachedCore()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        MapGameManager gameManager = FindFirstObjectByType<MapGameManager>();

        if (gameManager != null)
        {
            if (isBoss)
            {
                // Boss vào Core thì KHÔNG tính là boss chết
                // Không gọi RegisterBossDied ở đây
                Debug.Log("Boss reached the Core. This is not a win.");
            }
            else
            {
                // Enemy thường vào Core thì giảm số quái còn lại trong wave
                // Không cộng gold
                ReportRemovedFromWave(gameManager);
            }
        }

        Destroy(gameObject);
    }

    private void ReportRemovedFromWave(MapGameManager gameManager)
    {
        if (!isWaveEnemy)
        {
            Debug.LogWarning(gameObject.name + " is not a wave enemy, so it will not reduce wave counter.");
            return;
        }

        if (hasReportedRemoved)
        {
            return;
        }

        hasReportedRemoved = true;
        gameManager.RegisterEnemyRemoved();
    }
}