using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public float missionDuration;
    public float startMissionDuration;
    float secondTimer = 0f;

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
            FindObjectOfType<ScenesController>().GameOver();
        }
    }
}
