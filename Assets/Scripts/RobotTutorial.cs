using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotTutorial : AIRobotController
{
    private float waitTime;

    void Update()
    {
        CheckIfSpying();
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
        bool showRobot = TutorialManager.sharedInstance.showRobot;

        // Get all states except walking and flying
        var eligibleStates = states
            .Where(kvp => kvp.Key != "walking" && kvp.Key != "flying")
            .Select(kvp => kvp.Value)
            .ToList();

        if (eligibleStates.Count == 0) return null;

        // Build a filtered list of animations according to showRobot
        List<string> matchingAnims = new List<string>();

        foreach (var state in eligibleStates)
        {
            if (state.animations == null) continue;

            matchingAnims.AddRange(
                state.animations
                    .Select(a => a.Item2)
                    .Where(animName =>
                        showRobot
                            ? animName.StartsWith("R")    // Only R animations
                            : !animName.StartsWith("R")   // Only non-R animations
                    )
            );
        }
        Debug.Log("animR? " + showRobot);
        if (matchingAnims.Count == 0) return null;

        // Pick one at random
        return matchingAnims[Random.Range(0, matchingAnims.Count)];
    }


}
