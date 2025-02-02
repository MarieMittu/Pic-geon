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

    public float introTime;
    Scene currentScene;
    private string sceneName;

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
    }

    private void Update()
    {
        if (sceneName == "IntroScene")
        {
            Cursor.lockState = CursorLockMode.Locked;
            introTime -= Time.deltaTime;

            if (introTime <= 0) GoToMenu();
        }
    }

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

    public void ExitToMenu()
    {
        Debug.Log("pressing exit menu");
        Invoke("GoToMenu", 1f);
    }

    public void LoadNextLevel()
    {
        Debug.Log("pressing next lvl");
        switch (MissionManager.sharedInstance.currentMission)
        {
            case 2:
                Invoke("LoadMissionOne", 1f);
                //LoadMissionOne(); 
                break;
            case 3:
                Invoke("LoadMissionTwo", 1f);
                //LoadMissionTwo();
                break;
            case 4:
                Invoke("LoadMissionThree", 1f);
                //LoadMissionThree(); 
                break;
        }

        //Cursor.lockState = CursorLockMode.Locked;
        //Time.timeScale = 1f;
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
        SceneManager.LoadScene("NextLevelSceneV");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    public void GameWon()
    {
        SceneManager.LoadScene("WinScene");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    public void RetryMission()
    {
        switch (MissionManager.sharedInstance.currentMission)
            {
                case 1:
                Invoke("StartGame", 1f);
                break;
                case 2:
                Invoke("LoadMissionOne", 1f);
                break;
                case 3:
                Invoke("LoadMissionTwo", 1f);
                break;
                case 4:
                Invoke("LoadMissionThree", 1f);
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
