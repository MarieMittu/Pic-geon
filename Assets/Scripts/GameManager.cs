using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;

    public float missionDuration;
    public float startMissionDuration;
    float secondTimer = 0f;

    private void Awake()
    {
        sharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        startMissionDuration = missionDuration;   
    }

    // Update is called once per frame
    void Update()
    {
        secondTimer += Time.deltaTime;
        if (secondTimer >= 1f)
        {
            missionDuration--;
            secondTimer -= 1f;
        }
        
        if (missionDuration <= 0)
        {
            TriggerNextLevel(); //TODO: add condition for % of correct photos if not all used, PlayerPrefs for saving?
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
