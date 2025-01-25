using UnityEngine;
using UnityEngine.AI;

public class AIRobotController : AIBirdController
{
    [HideInInspector] public bool isSpying = false;

    public Vector3 rectangleSize = new Vector3(20, 0, 10);

    private void Update()
    {
        CheckIfSpying();
        var action = currentState.stateAction;
        if (action != null) action();
        PerformActionsSequence();
        if (shallWeLog)
        {
            shallWeLog = false;
            Debug.Log("State: " + currentState.name);
        }
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

        // states
        states["standing"] = new State("standing", new (float, string)[]{
            (1.0f, "01_Standing_Idle"),
            (1.0f, "01_Standing_Cleaning"),
            (1.0f, "R01_Standing_Idle_Antenna"),
            (1.0f, "R01_Standing_Idle_Head"),
            (1.0f, "R01_Standing_Idle_Shutter"),
            (1.0f, "R01_Standing_Idle_Tweak"),
        });

        states["sitting"] = new State("sitting", new (float, string)[]{
            (1.0f, "02_Sitting_Idle"),
            (1.0f, "R02_Sitting_Idle_Head"),
            (1.0f, "R02_Sitting_Idle_Tweak"),
            (1.0f, "R02_Sitting_Idle_Shutter"),
        });
        states["sleeping"] = new State("sleeping", new (float, string)[]{
            (1.0f, "02_Sitting_Sleeping_Idle"),
            (1.0f, "R02_Sitting_Sleeping_Antenna"),
            (1.0f, "R02_Sitting_Sleeping_Idle_Tweak"),
        });

        states["walking"] = new State("walking", new (float, string)[]{
            (1.0f, "03_Walking_Idle"),
            (1.0f, "R03_Walking_Idle_Tweak"),
            (1.0f, "R03_Walking_Picking_Shutter"),
            (1.0f, "R03_Walking_Picking_Stiff"),
            (1.0f, "R03_Walking_Idle_Shutter"),
        }, WalkAround);

        states["flying"] = new State("flying", new (float, string)[]{
            (1.0f, "03_Walking_Idle"),
        }, Fly);

        // state transitions
        states["standing"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{ "02_Sitting_Down" }, states["sitting"])),
            (1.0f, (new string[]{  }, states["walking"])),
            (1.0f, (new string[]{  }, states["flying"])),
        };

        states["sitting"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{ "02_Sitting_Standing_up" }, states["standing"])),
            (1.0f, (new string[]{ "02_Sitting_Falling_Asleep" }, states["sleeping"])),
        };
        states["sleeping"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{ "02_Sitting_Waking_up" }, states["sitting"])),
        };

        states["walking"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{  }, states["standing"])),
        };

        states["flying"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{  }, states["standing"])),
        };

        // starting state
        currentState = states["standing"];
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
            Gizmos.DrawSphere(agent.destination, 0.1f); // Visualize the agent's current destination
        }
    }

}
