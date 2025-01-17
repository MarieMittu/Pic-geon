using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRobotController : AIBirdController
{
    public bool isSpying = false;

    private void Update()
    {

        PerformActionsSequence();
    }

    public override void PerformRandomAction()
    {
        isSpying = false;
        List<RandomActions> randomActions = new List<RandomActions>
        {
            StandStill,
            CleanItself,
            () => SitDown(standUpAnim: "02_Sitting_Standing_up"),
            ShowAntenna,
            WalkAround,
            //Fly
        };

        randomActions[Random.Range(0, randomActions.Count)]();
    }

    public void ShowAntenna()
    {
        Sleep("R02_Sitting_Sleeping", "02_Sitting_Standing_up");
        isSpying = true;
    }
}
