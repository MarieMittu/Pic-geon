using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTutorial : AIRobotController
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TeachRobotic()
    {
        animator.CrossFade("R02_Sitting_Sleeping", 0.1f);
    }

    void ReturnToNormal()
    {
        animator.CrossFade("01_Standing_Idle_On_Stick", 0.1f);
    }
}
