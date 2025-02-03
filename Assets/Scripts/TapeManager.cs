using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TapeManager : MonoBehaviour
{

    public static TapeManager instance;

    public int tapeLimit;
    private int usedTape = 0;
    public Text tapeText;

    public bool reachedLimit = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
   
    }

    private void OnEnable()
    {
        Debug.Log("mission check " + MissionManager.sharedInstance.currentMission);

        if (MissionManager.sharedInstance.currentMission < 3) // reset in tutorial and level 1 (photos in tutorial don't count)
        {
            ResetTape();
        }
        else
        {
            usedTape = PlayerPrefs.GetInt("usedtape", usedTape);

        }

        tapeText.text = usedTape + "/" + tapeLimit;
    }

    public void AddUsedTape()
    {
        usedTape++;
        PlayerPrefs.SetInt("usedtape", usedTape);
        tapeText.text = usedTape + "/" + tapeLimit;

        if (usedTape == tapeLimit)
        {
            reachedLimit = true;
        }
    }

    public void ResetTape() //TODO: call it when the very first mission starts, need camera track
    {
        usedTape = 0;
        PlayerPrefs.SetInt("usedtape", usedTape);
        tapeText.text = usedTape + "/" + tapeLimit + " tape used";
    }
}
