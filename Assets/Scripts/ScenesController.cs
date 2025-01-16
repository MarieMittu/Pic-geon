using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesController : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject alert;
    [SerializeField] GameObject photosLibrary;
    [SerializeField] GameObject submitOption;

    public bool isPaused = false;
    public bool isAlerting = false;

    public void StartGame()
    {
        MissionManager.sharedInstance.SetMission(1);
        MissionManager.sharedInstance.StartSelectedMission();
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
        SceneManager.LoadScene("SampleScene");
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void LoadMissionTwo()
    {
        MissionManager.sharedInstance.SetMission(2);
        MissionManager.sharedInstance.StartSelectedMission();
        SceneManager.LoadScene("SampleScene");
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void LoadMissionThree()
    {
        MissionManager.sharedInstance.SetMission(3);
        MissionManager.sharedInstance.StartSelectedMission();
        SceneManager.LoadScene("SampleScene");
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
        if (photosLibrary.activeInHierarchy)
        {
            if (GameManager.sharedInstance.bigPhotoImage.IsActive())
            {
                GameManager.sharedInstance.bigPhoto.SetActive(false);
            } else
            {
                photosLibrary.SetActive(false);
                FindObjectOfType<MenusController>().currentMode = MenusController.MenuMode.OptionsMenu;
            }
            
        } else
        {
            if (alert.activeInHierarchy) alert.SetActive(false);
            isAlerting = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }

    public void ShowAlert()
    {
        alert.SetActive(true);
        isAlerting = true;
    }

    public void ActivateSubOption()
    {
        submitOption.SetActive(true);
    }

    public void ShowPhotoLibrary()
    {
        photosLibrary.SetActive(true);
    }
}
