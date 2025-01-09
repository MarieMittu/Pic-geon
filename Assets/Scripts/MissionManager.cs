using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    public static MissionManager sharedInstance;

    public int currentMission = 1;
    public int maxMissions = 3;

    public Camera camera1;
    public Camera camera2;
    public Camera camera3;

    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }

    public void SetMission(int mission)
    {
        if (mission > 0 && mission <= maxMissions)
        {
            currentMission = mission;
        }
    }

    public void NextMission()
    {
        if (currentMission < maxMissions)
        {
            currentMission++;
        }
        Debug.Log("mission " + currentMission);
    }
}
