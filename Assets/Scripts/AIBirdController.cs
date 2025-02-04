using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class AIBirdController : MonoBehaviour
{
    public float startRange;
    public float finalRange;

    private float actionTime; //waiting time between start of random actions
    private float stateTime; //waiting time between changing states
    private bool isTransitioning = false;
    public delegate void RandomActions();

    Rigidbody rb;

    public Animator animator;

    private bool blockStateTransition = false;
    private bool blockAutoAnimation = false;
    public bool shallWeLog = false;

    [HideInInspector] public NavMeshAgent agent;

    public Vector3 centrePoint = new Vector3(0, 0, 0); //point around which walking area is generated
    public Vector3 rectangleSize = new Vector3(20, 0, 10); // walking area
    public float walkRotationAngle;

    protected Dictionary<string, State> states = new Dictionary<string, State>();
    protected State currentState;

    protected class State
    {
        public string name;
        // (weight, (transition animations, state))
        public (float, (string[], State))[] transitions;
        // (weight, animation name)
        (float, string)[] animations;
        public RandomActions stateAction = null;

        public State(string name, (float, string)[] animations, RandomActions stateAction = null)
        {
            this.name = name;
            this.animations = animations;
            this.stateAction = stateAction;
        }

        public string GetRandomAnimation()
        {
            return ChooseRandomWeighted<string>(animations);
        }
        public (string, State) GetRandomTransition()
        {
            var trans = ChooseRandomWeighted(transitions);
            if (trans.Item1 == null || trans.Item1.Length == 0) return (null, trans.Item2);
            return (trans.Item1[Random.Range(0, trans.Item1.Length)], trans.Item2);
        }
        private static T ChooseRandomWeighted<T>((float, T)[] c)
        {
            float weightSum = 0;
            foreach (var e in c)
            {
                weightSum += e.Item1;
            }
            float rand = Random.Range(0, weightSum);
            for (int i = 0; i < c.Length; i++)
            {
                var e = c[i];
                rand -= e.Item1;
                if (rand <= 0)
                {
                    return c[i].Item2;
                }
            }
            return default;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        actionTime = 1;
        stateTime = 2;
        rb = gameObject.GetComponent<Rigidbody>();

        agent = GetComponent<NavMeshAgent>();

        InitializeStates();
    }

    protected virtual void InitializeStates()
    {
        // states
        states["standing"] = new State("standing", new (float, string)[]{
            (1.0f, "01_Standing_Idle"),
            (1.0f, "01_Standing_Cleaning"),
            (1.0f, "01_Standing_Picking_1"),
            (1.0f, "01_Standing_Picking_2"),
        });

        states["sitting"] = new State("sitting", new (float, string)[]{
            (1.0f, "02_Sitting_Idle"),
        });
        states["sleeping"] = new State("sleeping", new (float, string)[]{
            (1.0f, "02_Sitting_Sleeping_Idle"),
        });

        states["walking"] = new State("walking", new (float, string)[]{
            (1.0f, "03_Walking_Idle"),
            (1.0f, "03_Walking_Bending_Down_Picking_Bending_Up"),
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
            (1.0f, (new string[]{ "02_Sitting_Standing_up", "02_Sitting_Standing_Up_Picking" }, states["standing"])),
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

    private void Update()
    {
        var action = currentState.stateAction;
        if (action != null) action();
        PerformActionsSequence();
        if (shallWeLog)
        {
            shallWeLog = false;
            Debug.Log("State: " + currentState.name);
        }
    }

    public void PerformActionsSequence()
    {
        actionTime -= Time.deltaTime;
        stateTime -= Time.deltaTime;
        if (isTransitioning) return;
        if (stateTime <= 0 && !blockStateTransition)
        {
            TransitionToNextState();
            stateTime = Random.Range(startRange, finalRange);
            actionTime = 0;
        }
        else if (!blockAutoAnimation && (actionTime <= 0 || (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && animator.GetCurrentAnimatorClipInfo(0).Length != 0)))
        {
            string anim = PerformRandomAction();
            actionTime = ActionDuration(anim);
        }
    }

    public virtual string PerformRandomAction()
    {
        var anim = currentState.GetRandomAnimation();
        animator.CrossFade(anim, 0.1f);
        return anim;
    }

    protected void TransitionToNextState()
    {
        var trans = currentState.GetRandomTransition();
        var anim = trans.Item1;
        if(!string.IsNullOrEmpty(anim)) StartCoroutine(Transit(anim));
        currentState = trans.Item2;
        blockStateTransition = false;
    }

    // helpers

    private IEnumerator Transit(string anim, string nextAction = null)
    {
        if (isTransitioning) yield break;
        isTransitioning = true;
        animator.CrossFade(anim, 0.1f);

        float animationDuration = ActionDuration(anim);
        yield return new WaitForSeconds(animationDuration);

        isTransitioning = false;

        if (!string.IsNullOrEmpty(nextAction))
        {
            animator.CrossFade(nextAction, 0.1f);
        }
    }

    private float ActionDuration(string anim)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == anim)
            {
                return clip.length;
            }
        }
        return 1.0f;
    }

    // normal actions

    public void WalkAround()
    {
        // start new path
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) //done with path
        {
            if (blockStateTransition)
            {
                stateTime = 0;
                blockStateTransition = false;
                return;
            }
            Vector3 point;
            if (RandomPoint(centrePoint, rectangleSize, walkRotationAngle, out point)) // Pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.red, 2.0f); // So you can see the point with gizmos
                agent.SetDestination(point);
            }
        }
        // while on path
        else
        {
            blockStateTransition = true;
            DuringWalk();
        }
    }

    private void DuringWalk()
    {
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = agent.velocity.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    protected virtual bool RandomPoint(Vector3 center, Vector3 rectangleSize, float rotationAngle, out Vector3 result)
    {

        float halfX = rectangleSize.x / 2;
        float halfZ = rectangleSize.z / 2;

        float randomX = Random.Range(-halfX, halfX);
        float randomZ = Random.Range(-halfZ, halfZ);
        Vector3 localRandomPoint = new Vector3(randomX, 0, randomZ);

        // apply rotation to the rectangle
        Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
        Vector3 rotatedPoint = rotation * localRandomPoint;

        // transform to world space
        Vector3 worldPoint = center + rotatedPoint;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(worldPoint, out hit, 10.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            Debug.DrawRay(result, Vector3.up, Color.blue, 2.0f);
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Quaternion rotation = Quaternion.Euler(0, walkRotationAngle, 0);

        Gizmos.matrix = Matrix4x4.TRS(centrePoint, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, rectangleSize);

        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(agent.destination, 0.1f);
        }
    }

    Vector3 closestStart = Vector3.negativeInfinity;
    SplineContainer closestSpline;

    protected void Fly()
    {
        GameObject[] flightPaths = GameObject.FindGameObjectsWithTag("FlightPath");
        if (closestStart.Equals(Vector3.negativeInfinity))
        {
            closestStart = flightPaths[0].GetComponent<SplineContainer>().EvaluatePosition(0, 0f);
            closestSpline = flightPaths[0].GetComponent<SplineContainer>();
        }
        // just started fly state
        if (!blockStateTransition)
        {
            // get nearest flight path
            foreach (GameObject flightPath in flightPaths)
            {
                SplineContainer sc = flightPath.GetComponent<SplineContainer>();
                Vector3 startPos = sc.EvaluatePosition(0, 0f);
                if (Vector3.Distance(transform.position, startPos) < Vector3.Distance(transform.position, closestStart))
                {
                    closestStart = startPos;
                    closestSpline = sc;
                }
            }
            NavMeshHit hit;
            if (NavMesh.SamplePosition(closestStart, out hit, 1.0f, NavMesh.AllAreas))
            {
                Debug.DrawRay(hit.position, Vector3.up, Color.red, 1.0f); //so you can see with gizmos
                                                                          // starts moving to point
                agent.SetDestination(hit.position);
                if (Vector3.Distance(agent.destination, closestStart) > 1)
                {
                    ForceStateChange();
                    return;
                }
                blockStateTransition = true;
                stateTime = 20; // state time is used as a limit for how long the bird tries to get to the spline
                animator.CrossFade("03_Walking_Idle", 0.1f);
                //Debug.Log(name + "STARTING THE PATH: " + closestSpline.name);
            }
            else
            {
                ForceStateChange();
                return;
            }
        }
        else
        {
            blockAutoAnimation = true; // prevent automatic animation switch
            SplineAnimate splineAnim = GetComponent<SplineAnimate>();
            if (!splineAnim.enabled && (stateTime <= 0 || agent.pathStatus != NavMeshPathStatus.PathComplete))
            {
                //Debug.Log(name + "HAVE FAILED THE PATH: " + closestSpline.name);
                ForceStateChange();
            }
            else if (Vector3.Distance(transform.position, closestSpline.EvaluatePosition(0, 0f)) <= 0.5f && !splineAnim.enabled)
            {
                //Debug.Log(name + "AM AT THE PATH: " + closestSpline.name);
                // if destination was reached, start flight
                agent.ResetPath();
                agent.enabled = false;
                animator.CrossFade("04_Flying", 0.1f);
                splineAnim.Container = closestSpline;
                splineAnim.enabled = true;
                splineAnim.Play();
            }
            else if (splineAnim.enabled && splineAnim.NormalizedTime >= 1)
            {
                //Debug.Log(name + "HAVE LEFT THE PATH: " + closestSpline.name);
                // when flight along the spline is finished
                splineAnim.enabled = false;
                agent.enabled = true;
                agent.ResetPath();
                closestStart = Vector3.negativeInfinity;
                // force walking state to get the bird away from the landing zone
                blockStateTransition = false;
                Vector3 point;
                // smaller radius than normal for walking state
                if (RandomPoint(transform.position, new Vector3(2f, 0f, 2f), walkRotationAngle, out point))
                {
                    Debug.DrawRay(point, Vector3.up, Color.red, 2.0f); // So you can see the point with gizmos
                    agent.SetDestination(point);
                    blockStateTransition = true;
                }
                currentState = states["walking"];
                blockAutoAnimation = false;
                actionTime = 0f;
                stateTime = 5f;
            }
            else
            {
                //if (Input.GetKeyDown(KeyCode.P)) Debug.Log(name + "WALKING TO THE PATH: " + closestSpline.name);
            }
        }
    }

    private void ForceStateChange()
    {
        agent.ResetPath();
        stateTime = 0;
        blockStateTransition = false;
        blockAutoAnimation = false;
    }
}
