using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("SampleScene"); // TODO: add camera selection
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenIntermediateScene()
    {
        SceneManager.LoadScene("NextLevelScene");
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
}
