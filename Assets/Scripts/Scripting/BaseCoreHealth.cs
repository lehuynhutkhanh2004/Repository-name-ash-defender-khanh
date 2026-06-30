using TMPro;
using UnityEngine;

public class BaseCoreHealth : MonoBehaviour
{
    [Header("Core Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("UI")]
    public TMP_Text coreHPText;
    public GameObject losePanel;

    private bool isDestroyed = false;

    private void Awake()
    {
        currentHP = maxHP;
        UpdateUI();

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed)
        {
            return;
        }

        currentHP -= damage;

        if (currentHP < 0)
        {
            currentHP = 0;
        }

        Debug.Log("Core HP: " + currentHP + "/" + maxHP);

        UpdateUI();

        if (currentHP <= 0)
        {
            CoreDestroyed();
        }
    }

    private void UpdateUI()
    {
        if (coreHPText != null)
        {
            coreHPText.text = "Core HP: " + currentHP + "/" + maxHP;
        }
    }

    private void CoreDestroyed()
    {
        isDestroyed = true;

        Debug.Log("LOSE! Core destroyed!");

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}