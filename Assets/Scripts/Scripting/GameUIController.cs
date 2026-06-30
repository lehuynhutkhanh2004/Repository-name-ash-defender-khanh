using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject pausePanel;

    [Header("Spawner")]
    public MapWaveSpawner mapWaveSpawner;

    private bool gameStarted = false;
    private bool isPaused = false;

    private void Start()
    {
        Time.timeScale = 0f;

        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (mapWaveSpawner == null)
        {
            mapWaveSpawner = FindFirstObjectByType<MapWaveSpawner>();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        isPaused = false;

        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;

        if (mapWaveSpawner != null)
        {
            mapWaveSpawner.StartWaves();
        }
        else
        {
            Debug.LogError("MapWaveSpawner is missing!");
        }
    }

    public void PauseGame()
    {
        if (!gameStarted)
        {
            return;
        }

        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!gameStarted)
        {
            return;
        }

        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}