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
        int currentMission = MissionManager.sharedInstance.currentMission;

        if (currentMission > 1 && currentMission - 2 < newMissions.Length)
        {
            for (int i = 0; i <= currentMission - 2; i++)
            {
                newMissions[i].SetActive(true);
            }
        }
    }
}
