using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRobotController : AIBirdController
{
    public override void PerformRandomAction()
    {
        List<RandomActions> randomActions = new List<RandomActions>
        {
            PerformChirp,   
            PerformShit,   
            PerformEat,     
            PerformSpyAct,  
            ShowAntennae    
        };

        randomActions[Random.Range(0, randomActions.Count)]();
    }

    public void PerformSpyAct()
    {
        Debug.Log("I am watching u!");
    }

    public void ShowAntennae()
    {
        Debug.Log("Beep beep!");
    }
}
