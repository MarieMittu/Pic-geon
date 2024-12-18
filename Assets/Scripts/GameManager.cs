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
            }
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
}
