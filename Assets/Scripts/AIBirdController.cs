using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class AIBirdController : MonoBehaviour
{
    public float startRange;
    public float finalRange;

    private float actionTime; //waiting time between start of random actions
    private bool isTransitioning = false;
    public delegate void RandomActions();

    Rigidbody rb;

    public Animator animator;

    protected bool isSitting;
    private bool isWalking;
    private bool isFlying;
    private bool isSleeping;

    NavMeshAgent agent;
    public float walkRadius ; //radius of sphere to walk around

    public Vector3 centrePoint = new Vector3(0, 0, 0); //point around which bird walks

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        actionTime = 1;
        rb = gameObject.GetComponent<Rigidbody>();

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        PerformActionsSequence();
    }

    public void PerformActionsSequence()
    {
        if (isTransitioning) return;
        actionTime -= Time.deltaTime;
        if (actionTime <= 0)
        {
            PerformRandomAction();
            actionTime = Random.Range(startRange, finalRange);

        }
    }

    public virtual void PerformRandomAction()
    {

        List<RandomActions> randomActions = new List<RandomActions>
        {
            StandStill,
            CleanItself,
            () => SitDown(standUpAnim: Random.Range(0, 2) == 0 ? "02_Sitting_Standing_up" : "02_Sitting_Standing_Up_Picking"),
            () => Sleep(anim: "02_Sitting_Sleeping_Idle", standupAnim: Random.Range(0, 2) == 0 ? "02_Sitting_Standing_up" : "02_Sitting_Standing_Up_Picking"),
            PickFoodStanding,
            PickFoodWalking,
            WalkAround,
            //Fly
        };


        randomActions[Random.Range(0, randomActions.Count)]();
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

    public void StandStill()
    {
        if (!isWalking && !isFlying && !isSitting)
        {
            animator.CrossFade("01_Standing_Idle", 0.1f);
        }
        
    }

    public void CleanItself()
    {
        if (!isWalking && !isFlying && !isSitting)
        {
            animator.CrossFade("01_Standing_Cleaning", 0.1f);
        }
       
    }

    public void SitDown(string standUpAnim)
    {
        if (!isWalking && !isFlying && !isSitting)
        {
            StartCoroutine(StaySit(standUpAnim));
        }
       
    }

    private IEnumerator StaySit(string standUpAnim)
    {
        isSitting = true;
        yield return StartCoroutine(Transit("02_Sitting_Down", "02_Sitting_Idle"));

        if (!isSleeping) StartCoroutine(StandUp(standUpAnim));

    }

    private IEnumerator StandUp(string standUpAnim)
    {
            float waitTime = Random.Range(4f, 8f);
            yield return new WaitForSeconds(waitTime);

            //string standUpAnim = Random.Range(0, 2) == 0 ? "02_Sitting_Standing_up" : "02_Sitting_Standing_Up_Picking";
        yield return StartCoroutine(Transit(standUpAnim, "01_Standing_Idle"));
        isSitting = false;

    }

    public void PickFoodStanding()
    {
        if (!isWalking && !isFlying && !isSitting)
        {
            StartCoroutine(Transit("01_Standing_Picking_1", "01_Standing_Idle"));
        }
        
    }

    public void PickFoodWalking()
    {
        if (isWalking)
        {
            StartCoroutine(Transit("03_Walking_Bending_Down_Picking_Bending_Up", "03_Walking_Ilde"));
        }
        
    }


    public void Sleep(string anim, string standupAnim)
    {
        if (isSitting && !isSleeping) StartCoroutine(StayAsleep(anim, standupAnim));
    }

    private IEnumerator StayAsleep(string anim, string standupAnim)
    {
        isSleeping = true;
        yield return StartCoroutine(Transit("02_Sitting_Falling_Asleep"));

        animator.CrossFade(anim, 0.1f);

        yield return new WaitForSeconds(Random.Range(4f, 8f)); 

        yield return StartCoroutine(Transit("02_Sitting_Waking_up"));
        isSleeping = false;

        animator.CrossFade("02_Sitting_Idle", 0.1f);

        if (!isSleeping) StartCoroutine(StandUp(standupAnim));
    }



    public void WalkAround()
    {
        if (!isWalking && !isSitting && !isFlying && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) //done with path
        {
            Vector3 point;
            if (RandomPoint(centrePoint, walkRadius, out point)) // Pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.red, 2.0f); // So you can see the point with gizmos
                StartCoroutine(RotateAndMoveToPoint(point));
            }
        }

    }

    private IEnumerator RotateAndMoveToPoint(Vector3 targetPoint)
    {
        isWalking = true;

        Vector3 direction = (targetPoint - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float rotationSpeed = 1f;
        float walkThreshold = 0.1f;

        agent.SetDestination(targetPoint);
        animator.CrossFade("03_Walking_Ilde", 0.1f);

      
        
        while (agent.remainingDistance > agent.stoppingDistance || agent.pathPending)
        {
            direction = (targetPoint - transform.position).normalized; // Recalculate direction to account for movement
            targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (Quaternion.Angle(transform.rotation, targetRotation) < walkThreshold)
            {
                //animator.CrossFade("03_Walking_Ilde", 0.1f);
            }
            yield return null;
        }
        
        isWalking = false;
        StandStill();
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
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
        if (!isSitting)
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
            if (Vector3.Distance(startPos, transform.position) < 0.5) // this number may need to change once in a real scene
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(startPos, out hit, 1.0f, NavMesh.AllAreas))
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
        float maxDuration = 20;
        float timer = 0f;
        animator.Play("03_Walking_Ilde");
        // wait until destination is reached or timer runs out
        while (timer < maxDuration && Vector3.Distance(agent.destination, transform.position) >= 0.1)
        {

            timer += Time.deltaTime;
            yield return null;
        }

        // if destination was reached, start flight
        if (Vector3.Distance(agent.destination, transform.position) <= 0.1)
        {
            animator.Play("04_Flying");
            isFlying = true;
            SplineAnimate splineAnim = GetComponent<SplineAnimate>();
            splineAnim.Container = splineContainer;
            splineAnim.enabled = true;
            splineAnim.Play();
        }
        isFlying = false;
        // change to courutine take off - fly - land
        //animator.Play("03_Walking_Ilde");
    }
}
