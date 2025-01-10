using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] GameObject[] newMissions;

    private void Awake()
    {
        ActivateNextMission();
    }

    public void ActivateNextMission()
    {
        for (int i = 0; i < newMissions.Length; i++)
        {
            int missionNum = i + 2; 
            newMissions[i].SetActive(MissionManager.sharedInstance.IsMissionUnlocked(missionNum));
        }
    }
}
