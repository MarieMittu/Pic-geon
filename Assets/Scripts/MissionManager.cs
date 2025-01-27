using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    public static MissionManager sharedInstance;

    public int currentMission = 1;
    public int maxMissions = 4;
    public int selectedMission = 1;

    public int[] requiredPhotosForMissions = { 1, 2, 3, 4 };

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
            Debug.Log($"Mission {mission} selected.");
        }
        else
        {
            Debug.LogWarning($"Mission {mission} cannot be selected because it is not unlocked.");
        }
    }

    public void StartSelectedMission()
    {
        if (IsMissionUnlocked(selectedMission))
        {
            currentMission = selectedMission;
            Debug.Log($"Starting mission {currentMission}.");
        }
        else
        {
            Debug.LogWarning($"Cannot start mission {selectedMission} because it is not unlocked.");
        }

    }

    public void NextMission()
    {
        isTutorial = false;
        if (currentMission < maxMissions)
        {
            currentMission++;
            UnlockMission(currentMission);
            Debug.Log($"Advanced to next mission: {currentMission}.");

        }
        else
        {
            Debug.LogWarning("No more missions to unlock. Reached maxMissions.");
        }
        Debug.Log("mission " + currentMission);
    }

    public void UnlockMission(int mission)
    {
        if (mission > 0 && mission <= maxMissions)
        {
            if (!unlockedMissions.Contains(mission))
            {
                unlockedMissions.Add(mission);
                Debug.Log($"Mission {mission} unlocked.");
            }
        }
        else
        {
            Debug.LogWarning($"Mission {mission} is out of range (1 to {maxMissions}).");
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

    public int GetRequiredPhotos()
    {
        return requiredPhotosForMissions[currentMission - 1];
    }
}
