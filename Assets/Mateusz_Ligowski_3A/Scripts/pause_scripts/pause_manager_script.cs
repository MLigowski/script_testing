using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public int pauseSceneIndex = 2;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        SceneManager.LoadScene(pauseSceneIndex, LoadSceneMode.Additive);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        Debug.Log("RESUME FROM ESC");
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(pauseSceneIndex);
    }

    public void ResumeFromButton()
    {
        Debug.Log("Unfreeze button");
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(pauseSceneIndex);
    }

    public void BackToMainMenu()
    {
        Debug.Log("Back to main menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
