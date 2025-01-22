using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    public static MissionManager sharedInstance;

    public int currentMission = 1;
    public int maxMissions = 4;
    public int selectedMission = 1;

    private HashSet<int> unlockedMissions = new HashSet<int> { 1 };

    public bool isTutorial = false;

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
        if (IsMissionUnlocked(mission))
        {
            selectedMission = mission;
        }
    }

    public void StartSelectedMission()
    {
        if (IsMissionUnlocked(selectedMission))
        {
            currentMission = selectedMission;
        }
            
    }

    public void NextMission()
    {
        isTutorial = false;
        if (currentMission < maxMissions)
        {
            currentMission++;
            UnlockMission(currentMission);
        }
        Debug.Log("mission " + currentMission);
    }

    public void UnlockMission(int mission)
    {
        if (mission > 0 && mission <= maxMissions)
        {
            unlockedMissions.Add(mission);
        }
    }

    public bool IsMissionUnlocked(int mission)
    {
        return unlockedMissions.Contains(mission);
    }

    public List<int> GetUnlockedMissions()
    {
        return new List<int>(unlockedMissions);
    }
}
