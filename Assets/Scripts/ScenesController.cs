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
        SceneManager.LoadScene("TutorialScene");
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
        switch (MissionManager.sharedInstance.currentMission)
        {
            case 2:
                LoadMissionOne(); 
                break;
            case 3:
                LoadMissionTwo();
                break;
            case 4:
                LoadMissionThree(); 
                break;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void LoadMissionOne()
    {
        MissionManager.sharedInstance.SetMission(2);
        MissionManager.sharedInstance.StartSelectedMission();
        SceneManager.LoadScene("LevelOne");
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void LoadMissionTwo()
    {
        MissionManager.sharedInstance.SetMission(3);
        MissionManager.sharedInstance.StartSelectedMission();
        SceneManager.LoadScene("LevelTwo");
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void LoadMissionThree()
    {
        MissionManager.sharedInstance.SetMission(4);
        MissionManager.sharedInstance.StartSelectedMission();
        SceneManager.LoadScene("LevelThree"); 
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

    public void GameWon()
    {
        SceneManager.LoadScene("WinScene");
        Cursor.lockState = CursorLockMode.None;
    }

    public void RetryMission()
    {
        switch (MissionManager.sharedInstance.currentMission)
            {
                case 1:
                Invoke("StartGame", 0.6f);
                break;
                case 2:
                Invoke("LoadMissionOne", 0.6f);
                break;
                case 3:
                Invoke("LoadMissionTwo", 0.6f);
                break;
                case 4:
                Invoke("LoadMissionThree", 0.6f);
                break;
            }
             
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

    public void ShowExitAlert()
    {
        GameManager.sharedInstance.wantsToExit = true;
        alert.SetActive(true);
        isAlerting = true;
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
