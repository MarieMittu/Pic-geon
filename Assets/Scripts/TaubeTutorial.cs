using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        string anim = PerformRandomAction();
        if (!string.IsNullOrEmpty(anim))
        {
            animator.CrossFade(anim, 0.1f);
        }

    }

    public override string PerformRandomAction()
    {
        // Get all states except walking and flying
        var eligibleStates = states
            .Where(kvp => kvp.Key != "walking" && kvp.Key != "flying")
            .Select(kvp => kvp.Value)
            .ToList();

        if (eligibleStates.Count == 0) return null;

        // Pick a random state weighted by number of animations (or just uniformly)
        State chosenState = eligibleStates[Random.Range(0, eligibleStates.Count)];

        // Get random animation from chosen state
        var anim = chosenState.GetRandomAnimation();

        return anim;
    }

    //void StandOnStick()
    //{
    //    animator.CrossFade("01_Standing_Idle_On_Stick", 0.1f);
    //}

    //void CleanOnStick()
    //{
    //    animator.CrossFade("01_Standing_Idle_On_Stick_Cleaning", 0.1f);
    //}
}
