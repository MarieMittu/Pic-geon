using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] GameObject[] newMissions;

    private void Start()
    {
        ActivateNextMission();
    }

    public void ActivateNextMission()
    {
        for (int i = 0; i < newMissions.Length; i++)
        {
            int missionNum = i + 2;
            bool isUnlocked = MissionManager.sharedInstance.IsMissionUnlocked(missionNum);

            newMissions[i].SetActive(isUnlocked);

            // Detailed debug log for each mission
            Debug.Log($"Mission {missionNum} ({newMissions[i].name}) - " +
                      $"Unlocked: {isUnlocked}, Current Mission: {MissionManager.sharedInstance.currentMission}");
        }

    }
}
