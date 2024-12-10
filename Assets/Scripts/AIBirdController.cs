using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBirdController : MonoBehaviour
{
    public float startRange;
    public float finalRange;

    private float actionTime; //waiting time between start of random actions
    public delegate void RandomActions();

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
    }

    public void PerformShit()
    {
        Debug.Log("Poops ahoy!");
    }

    public void PerformEat()
    {
        Debug.Log("I am eating!");
    }
}
