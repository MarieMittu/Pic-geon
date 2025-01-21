using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRobotController : AIBirdController
{
    public bool isSpying = false;

    private void Update()
    {
        string anim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (anim.StartsWith('R')) isSpying = true;
        else isSpying = false;
        PerformActionsSequence();
    }

    protected override void InitializeStates()
    {
        RandomActions sitDownAction = () => SitDown(standUpAnim: "02_Sitting_Standing_up");
        RandomActions sleepAction = () => Sleep(anim: "02_Sitting_Sleeping_Idle", standupAnim: "02_Sitting_Standing_up");
        
        currentState = StandStill;
        states[StandStill] = new List<System.Tuple<float, RandomActions>> {
            new System.Tuple<float, RandomActions>(0.05f, StandStill),
            new System.Tuple<float, RandomActions>(0.2f, CleanItself),
            new System.Tuple<float, RandomActions>(0.8f, sitDownAction),
            new System.Tuple<float, RandomActions>(0.15f, WalkAround),
            //new System.Tuple<float, RandomActions>(0.0f, Fly),
        };
        states[CleanItself] = new List<System.Tuple<float, RandomActions>> {
            new System.Tuple<float, RandomActions>(0.2f, StandStill),
            new System.Tuple<float, RandomActions>(0.05f, CleanItself),
            new System.Tuple<float, RandomActions>(0.7f, sitDownAction),
            new System.Tuple<float, RandomActions>(0.2f, WalkAround),
            //new System.Tuple<float, RandomActions>(0.0f, Fly),
        };
        states[sitDownAction] = new List<System.Tuple<float, RandomActions>> {
            new System.Tuple<float, RandomActions>(0.9f, ShowAntenna),
            new System.Tuple<float, RandomActions>(0.3f, StandStill),
        };
        states[WalkAround] = new List<System.Tuple<float, RandomActions>> {
            new System.Tuple<float, RandomActions>(0.2f, StandStill),
            new System.Tuple<float, RandomActions>(0.2f, CleanItself),
            new System.Tuple<float, RandomActions>(0.7f, sitDownAction),
            new System.Tuple<float, RandomActions>(0.15f, WalkAround),
            //new System.Tuple<float, RandomActions>(0.0f, Fly),
        };
        states[ShowAntenna] = new List<System.Tuple<float, RandomActions>> {
            new System.Tuple<float, RandomActions>(0.5f, ShowAntenna),
            new System.Tuple<float, RandomActions>(0.3f, StandStill),
        };
        //states[Fly] = new List<System.Tuple<float, RandomActions>> {
        //    new System.Tuple<float, RandomActions>(0.2f, StandStill),
        //    new System.Tuple<float, RandomActions>(0.2f, CleanItself),
        //    new System.Tuple<float, RandomActions>(0.4f, sitDownAction),
        //    new System.Tuple<float, RandomActions>(0.0f, ShowAntenna),
        //    new System.Tuple<float, RandomActions>(0.3f, WalkAround),
        //    //new System.Tuple<float, RandomActions>(0.0f, Fly),
        //};
    }

    public void ShowAntenna()
    {
        Sleep("R02_Sitting_Sleeping", "02_Sitting_Standing_up");
        isSpying = true;
    }
}
