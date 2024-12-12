using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBirdController : MonoBehaviour
{
    public float startRange;
    public float finalRange;

    private float actionTime; //waiting time between start of random actions
    public delegate void RandomActions();

    // for testing
    public float maxSize;
    public float growFactor;
    public float waitTime;

    public float y;
    public float speed;

    private void Start()
    {
        actionTime = 1;
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
        Debug.Log("Chirp! Chirp!");
        StartCoroutine(Scale());

    }

    public void PerformShit()
    {
        Debug.Log("Poops ahoy!");
        TestJump();
    }

    public void PerformEat()
    {
        Debug.Log("I am eating!");
        StartCoroutine(Spin());
    }

    // test actions
    IEnumerator Scale()
    {
        float timer = 0;

        while (true) // this could also be a condition indicating "alive or dead"
        {
            // we scale all axis, so they will have the same value, 
            // so we can work with a float instead of comparing vectors
            while (maxSize > transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }
            // reset the timer

            yield return new WaitForSeconds(waitTime);

            timer = 0;
            while (1 < transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }

            timer = 0;
            yield return new WaitForSeconds(waitTime);
        }
    }

    void TestJump()
    {
        if (y >= 3)
        {
            y = y - speed * Time.deltaTime;
        }
        if (y < 3)
        {
            y = y + speed * Time.deltaTime;
        }
        transform.position = new Vector3(0, y, 0);
    }

    IEnumerator Spin()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            float timer = 0f;
            while (timer < 1)
            {
                transform.Rotate(0, 0, 180 * Time.deltaTime);
                timer = timer + Time.deltaTime;
                yield return null;
            }
        }
    }
}
