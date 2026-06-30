using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultButtons : MonoBehaviour
{
    [Header("Next Scene")]
    public string nextSceneName = "Map_04";

    public void ReplayCurrentMap()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadNextMap()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("Next scene name is empty!");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}