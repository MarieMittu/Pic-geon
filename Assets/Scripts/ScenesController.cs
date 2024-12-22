using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesController : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject alert;

    public bool isPaused = false;

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
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
        Time.timeScale = 1f;
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
        if (alert.activeInHierarchy) alert.SetActive(false);
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

    public void ShowAlert()
    {
        alert.SetActive(true);
    }

    public void ActivateSubButton()
    {
        Button submitBtn = GameObject.Find("SubmitBtn").GetComponent<Button>();
        submitBtn.interactable = true;
    }
}
