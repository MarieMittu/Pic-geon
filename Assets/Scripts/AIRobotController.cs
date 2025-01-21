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
