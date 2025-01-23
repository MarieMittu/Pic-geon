using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIRobotController : AIBirdController
{
    [HideInInspector] public bool isSpying = false;

    public Vector3 rectangleSize = new Vector3(20, 0, 10);

    private void Update()
    {
        CheckIfSpying();
        PerformActionsSequence();
    }

    protected void CheckIfSpying()
    {
        try {
            string anim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (anim.StartsWith('R')) isSpying = true;
            else isSpying = false;
        }
        catch { 
            isSpying = false;
        }
    }

    protected override void InitializeStates()
    {
        RandomActions sitDownAction = () => SitDown(standUpAnim: "02_Sitting_Standing_up");
        RandomActions sleepAction = () => Sleep(anim: "02_Sitting_Sleeping_Idle", standupAnim: "02_Sitting_Standing_up");
        
        currentState = StandStill;
        states[StandStill] = new List<System.Tuple<float, RandomActions>> {
            new System.Tuple<float, RandomActions>(0.05f, TweakStanding),
            //new System.Tuple<float, RandomActions>(0.05f, StandStill),
            new System.Tuple<float, RandomActions>(0.2f, CleanItself),
            new System.Tuple<float, RandomActions>(0.8f, sitDownAction),
            new System.Tuple<float, RandomActions>(0.15f, WalkAround),
            new System.Tuple<float, RandomActions>(0.1f, Fly),
        };
        states[CleanItself] = new List<System.Tuple<float, RandomActions>> {
            new System.Tuple<float, RandomActions>(0.2f, StandStill),
            new System.Tuple<float, RandomActions>(0.05f, CleanItself),
            new System.Tuple<float, RandomActions>(0.7f, sitDownAction),
            new System.Tuple<float, RandomActions>(0.2f, WalkAround),
            new System.Tuple<float, RandomActions>(0.1f, Fly),
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
            new System.Tuple<float, RandomActions>(0.2f, Fly),
        };
        states[ShowAntenna] = new List<System.Tuple<float, RandomActions>> {
            new System.Tuple<float, RandomActions>(0.5f, ShowAntenna),
            new System.Tuple<float, RandomActions>(0.3f, StandStill),
        };
        states[Fly] = new List<System.Tuple<float, RandomActions>> {
            new System.Tuple<float, RandomActions>(0.2f, StandStill),
            new System.Tuple<float, RandomActions>(0.2f, CleanItself),
            new System.Tuple<float, RandomActions>(0.4f, sitDownAction),
            new System.Tuple<float, RandomActions>(0.0f, ShowAntenna),
            new System.Tuple<float, RandomActions>(0.3f, WalkAround),
            new System.Tuple<float, RandomActions>(0.05f, Fly),
        };
    }

    public void ShowAntenna()
    {
        Sleep("R02_Sitting_Sleeping", "02_Sitting_Standing_up");
        isSpying = true;
    }

    public void TweakStanding()
    {
          animator.CrossFade("R01_Standing_Idle_Antenna", 0.1f);
        
    }

    protected override bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        float halfX = rectangleSize.x / 2;
        float halfZ = rectangleSize.z / 2;

        float randomX = Random.Range(-halfX, halfX);
        float randomZ = Random.Range(-halfZ, halfZ);
        Vector3 randomPoint = new Vector3(center.x + randomX, center.y, center.z + randomZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(centrePoint, rectangleSize);

         if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.magenta; // Destination point color
            Gizmos.DrawSphere(agent.destination, 0.5f); // Visualize the agent's current destination
        }
    }

}
