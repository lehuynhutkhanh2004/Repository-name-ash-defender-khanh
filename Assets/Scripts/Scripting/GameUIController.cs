using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject pausePanel;
    public Button startButton;

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

        Debug.Log(123);

    }

    public void StartGame()
    {
        gameStarted = true;
        isPaused = false;

        Debug.Log(123);

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

        Debug.Log(000);

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