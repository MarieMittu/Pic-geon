using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTutorial : AIRobotController
{
    // Start is called before the first frame update
    void Start()
    {
        ReturnToNormal();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfSpying();

        if (TutorialManager.sharedInstance.showRobot)
        {
            TeachRobotic();
        } else
        {
            ReturnToNormal();
        }
    }

    void TeachRobotic()
    {
        if (!animator.IsInTransition(0) && !isSpying)
        {
            animator.CrossFade("R02_Sitting_Sleeping", 0.1f);
        }
    }

    void ReturnToNormal()
    {
        if (!animator.IsInTransition(0) && isSpying)
        {
            animator.CrossFade("01_Standing_Idle_On_Stick", 0.1f);
        }
    }
}
