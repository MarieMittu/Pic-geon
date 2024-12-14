using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBirdController : MonoBehaviour
{
    public float startRange;
    public float finalRange;

    private float actionTime; //waiting time between start of random actions
    public delegate void RandomActions();

    Rigidbody rb;

    // for testing
    public float pulseScale;
    public float pulseDuration;
    public int pulseCount;

    private Vector3 originalScale;

    public float velocity;
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
            PerformShit,
            PerformEat
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
            rb.AddForce(0, velocity, 0);
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
}
