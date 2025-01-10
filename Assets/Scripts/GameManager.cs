using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;

    public float missionDuration;
    public float startMissionDuration;
    float secondTimer = 0f;
    public Image timer;

    public bool isGamePaused;
    public bool hasEvidence;
    public bool hasCorrectPhotos;

    public Camera camera1;
    public Camera camera2;
    public Camera camera3;

    private void Awake()
    {

            sharedInstance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        startMissionDuration = missionDuration;
        if (timer != null)
        {
            timer.fillAmount = 1f;
        }
        hasEvidence = false;
        hasCorrectPhotos = false;
        SetUpCameras();
    }

    // Update is called once per frame
    void Update()
    {
        isGamePaused = FindObjectOfType<ScenesController>().isPaused;

        secondTimer += Time.deltaTime;
        if (secondTimer >= 1f)
        {
            missionDuration--;
            secondTimer -= 1f;

            if (timer != null)
            {
                timer.fillAmount = missionDuration / startMissionDuration;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FindObjectOfType<ScenesController>().isPaused)
            {
                FindObjectOfType<ScenesController>().Resume();
                Cursor.lockState = CursorLockMode.Locked;
            } else
            {
                FindObjectOfType<ScenesController>().Pause();
                if (hasEvidence)
                {
                    FindObjectOfType<ScenesController>().ActivateSubOption();
                } 
            }
        }

        FindObjectOfType<MenusController>().isHorizontal = FindObjectOfType<ScenesController>().isAlerting;
        if (FindObjectOfType<MenusController>().isHorizontal)
        {
            FindObjectOfType<MenusController>().SetupOptions();
        }
    }

    private void SetUpCameras()
    {
        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(false);
        camera3.gameObject.SetActive(false);

        switch (MissionManager.sharedInstance.currentMission)
        {
            case 1:
                camera1.gameObject.SetActive(true);
                break;
            case 2:
                camera2.gameObject.SetActive(true);
                break;
            case 3:
                camera3.gameObject.SetActive(true);
                break;
        }
    }


    public void TriggerGameOver()
    {
        FindObjectOfType<ScenesController>().GameOver();
    }

    public void TriggerNextLevel()
    {
        FindObjectOfType<ScenesController>().OpenIntermediateScene();
    }

   public void ControlEvidence()
    {
        if (hasCorrectPhotos)
        {
            MissionManager.sharedInstance.NextMission();
            TriggerNextLevel();
        } else
        {
            TriggerGameOver();
        }
    }

  
}
