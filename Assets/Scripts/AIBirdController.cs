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

    private bool isWalking;
    public bool shallWeLog = false;

    [HideInInspector] public NavMeshAgent agent;
    public float walkRadius ; //radius of sphere to walk around

    public Vector3 centrePoint = new Vector3(0, 0, 0); //point around which bird walks

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
            (1.0f, "03_Walking_Ilde"),
            (1.0f, "03_Walking_Bending_Down_Picking_Bending_Up"),
        }, WalkAround);

        // state transitions
        states["standing"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{ "02_Sitting_Down" }, states["sitting"])),
            (1.0f, (new string[]{  }, states["walking"])),
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

        // starting state
        currentState = states["standing"];
    }

    private void Update()
    {
        var action = currentState.stateAction;
        if (action != null) action();
        PerformActionsSequence();
    }

    public void PerformActionsSequence()
    {
        actionTime -= Time.deltaTime;
        stateTime -= Time.deltaTime;
        if (isTransitioning) return;
        if (stateTime <= 0 && !isWalking)
        {
            TransitionToNextState();
            stateTime = Random.Range(startRange, finalRange);
            actionTime = 0;
        }
        else if (actionTime <= 0 || (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && animator.GetCurrentAnimatorClipInfo(0).Length != 0))
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
            if (isWalking)
            {
                stateTime = 0;
                isWalking = false;
                return;
            }
            Vector3 point;
            if (RandomPoint(centrePoint, walkRadius, out point)) // Pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.red, 2.0f); // So you can see the point with gizmos
                agent.SetDestination(point);
            }
        }
        // while on path
        else
        {
            isWalking = true;
            if (agent.velocity.sqrMagnitude > 0.01f)
            {
                Vector3 moveDirection = agent.velocity.normalized;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); 
            }
        }
    }

    protected virtual bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
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
        // Draw the walkable area as a circle
        Gizmos.color = Color.red; // Circle color
        Gizmos.DrawWireSphere(centrePoint, walkRadius); // Visualize the walk radius from the center point

        // Optional: Draw the agent's current destination if the agent is moving
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.red; // Destination point color
            Gizmos.DrawSphere(agent.destination, 0.5f); // Visualize the agent's current destination
        }
    }


    public void Fly()
    {
        //if (!isSitting)
        StartCoroutine(FlyPreparation());
    }

    private IEnumerator FlyPreparation()
    {
        // check if a flight path spline starts near this bird
        GameObject[] flightPaths = GameObject.FindGameObjectsWithTag("FlightPath");
        foreach (GameObject flightPath in flightPaths)
        {
            SplineContainer sc = flightPath.GetComponent<SplineContainer>();
            Vector3 startPos = sc.EvaluatePosition(0, 0f);
            if (Vector3.Distance(startPos, transform.position) < 10) // this number may need to change once in a real scene
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(startPos, out hit, 15.0f, NavMesh.AllAreas))
                {
                    Debug.DrawRay(hit.position, Vector3.up, Color.red, 1.0f); //so you can see with gizmos
                    // starts moving to point

                    Vector3 direction = (hit.position - transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    float rotationSpeed = 5f;
                    while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                        yield return null;
                    }


                    agent.SetDestination(hit.position);
                    StartCoroutine(GetToFlightPathAndStartFlight(sc));
                }
            }
        }
    }

    IEnumerator GetToFlightPathAndStartFlight(SplineContainer splineContainer)
    {
        //isWalking = true;
        float maxDuration = 20;
        float timer = 0f;
        animator.Play("03_Walking_Ilde");
        // wait until destination is reached or timer runs out
        while (timer < maxDuration && Vector3.Distance(agent.destination, transform.position) >= 0.5)
        {

            timer += Time.deltaTime;
            yield return null;
        }

        SplineAnimate splineAnim = GetComponent<SplineAnimate>();
        // if destination was reached, start flight
        if (Vector3.Distance(agent.destination, transform.position) <= 0.5)
        {
            agent.ResetPath();
            agent.enabled = false;
            animator.CrossFade("04_Flying", 0.1f);
            //isFlying = true;
            splineAnim.Container = splineContainer;
            splineAnim.enabled = true;
            //isWalking = false;
            splineAnim.Play();
        }
        while (splineAnim.NormalizedTime < 1)
            yield return new WaitForSeconds(0.01f);
        agent.enabled = true;
        //isFlying = false;
        //isWalking = false;
        // change to courutine take off - fly - land
        animator.CrossFade("01_Standing_Idle", 0.2f);
    }
}
