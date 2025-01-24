using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaubeTutorial : AIBirdController
{
    private float waitTime;

    void Update()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0)
        {
            PerformRandomActions();
            waitTime = Random.Range(startRange, finalRange);

        }
    }


    void PerformRandomActions()
    {
        int action = Random.Range(0, 2); 

            if (action == 0)
            {
                StandOnStick();
            }
            else
            {
                CleanOnStick();
            }
        
    }

    void StandOnStick()
    {
        animator.CrossFade("01_Standing_Idle_On_Stick", 0.1f);
    }

    void CleanOnStick()
    {
        animator.CrossFade("01_Standing_Idle_On_Stick_Cleaning", 0.1f);
    }
}
