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
    public delegate void RandomActions();

    Rigidbody rb;

    NavMeshAgent agent;
    public float range; //radius of sphere to walk around

    public Vector3 centrePoint = new Vector3(0, 0, 0); //point around which bird walks

    // for testing
    public float pulseScale;
    public float pulseDuration;
    public int pulseCount;

    private Vector3 originalScale;

    public float jumpVelocity;
    public int maxJumps;
    private int currentJumps = 0;
    private bool isJumping = false;

    private void Start()
    {
        actionTime = 1;
        rb = gameObject.GetComponent<Rigidbody>();
        originalScale = transform.localScale;

        var cubeRenderer = GetComponent<Renderer>();
        cubeRenderer.material.SetColor("_Color", Color.blue);

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        PerformActionsSequence();
    }

    public void PerformActionsSequence()
    {
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
            PerformChirp,
            //PerformShit,
            PerformEat,
            WalkAround,
            Fly
        };


        randomActions[Random.Range(0, randomActions.Count)]();
    }

    public void PerformChirp()
    {
        //Debug.Log("Chirp! Chirp!");
        StartCoroutine(Pulse());

    }

    public void PerformShit()
    {
        //Debug.Log("Poops ahoy!");
        if (!isJumping)
        {
            StartCoroutine(TestJump());
        }
    }

    public void PerformEat()
    {
        //Debug.Log("I am eating!");
        StartCoroutine(Spin());
    }

    // test actions

    IEnumerator Pulse()
    {
        for (int i = 0; i < pulseCount; i++)
        {
            yield return Scale();
        }
    }

    IEnumerator Scale()
    {
        float elapsedTime = 0f;

        while (elapsedTime < pulseDuration / 2)
        {
            transform.localScale = Vector3.Lerp(originalScale, originalScale * pulseScale, elapsedTime / (pulseDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        transform.localScale = originalScale * pulseScale;

        elapsedTime = 0f;
        while (elapsedTime < pulseDuration / 2)
        {
            transform.localScale = Vector3.Lerp(originalScale * pulseScale, originalScale, elapsedTime / (pulseDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        transform.localScale = originalScale;
    }

    IEnumerator TestJump()
    {
        isJumping = true; 
        currentJumps = 0;

        while (currentJumps < maxJumps)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(0, jumpVelocity, 0);
            currentJumps++;

            yield return new WaitForSeconds(0.5f);
        }
        isJumping = false;
    }


    IEnumerator Spin()
    {
        float spinDuration = 3f; 
        float timer = 0f;

        while (timer < spinDuration)
        {
            
                transform.Rotate(0, 0, 180 * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            
        }
    }

    public void WalkAround()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(centrePoint, range, out point)) //pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.red, 1.0f); //so you can see with gizmos
                agent.SetDestination(point);
            }
        }
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
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public void Fly()
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

        // wait until destination is reached or timer runs out
        while (timer < maxDuration && Vector3.Distance(agent.destination, transform.position) >= 0.1)
        {

            timer += Time.deltaTime;
            yield return null;

        }

        // if destination was reached, start flight
        if (Vector3.Distance(agent.destination, transform.position) <= 0.1)
        {
            SplineAnimate splineAnim = GetComponent<SplineAnimate>();
            splineAnim.Container = splineContainer;
            splineAnim.enabled = true;
            splineAnim.Play();
        }
    }
}
