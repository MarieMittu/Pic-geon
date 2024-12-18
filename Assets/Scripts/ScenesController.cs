using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;

    public bool isPaused = false;

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("SampleScene"); // TODO: add camera selection
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenIntermediateScene()
    {
        SceneManager.LoadScene("NextLevelScene");
        Cursor.lockState = CursorLockMode.None;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
        Cursor.lockState = CursorLockMode.None;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
