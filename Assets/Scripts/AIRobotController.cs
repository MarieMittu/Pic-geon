using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRobotController : AIBirdController
{
    public float minSize;
    public bool isSpying = false;

    public override void PerformRandomAction()
    {
        List<RandomActions> randomActions = new List<RandomActions>
        {
            PerformChirp,
            PerformShit,
            PerformEat,     
            PerformSpyAct,  
            ShowAntennae    
        };

        randomActions[Random.Range(0, randomActions.Count)]();
    }

    public void PerformSpyAct()
    {
        //Debug.Log("I am watching u!");
        //StartCoroutine(MakeSmall());
    }

    public void ShowAntennae()
    {
        //Debug.Log("Beep beep!");
        StartCoroutine(ChangeColor());
    }

    //test actions
    IEnumerator ChangeColor()
    {
        var cubeRenderer = GetComponent<Renderer>();

        while (true)
        {
            cubeRenderer.material.SetColor("_Color", Color.blue);
            isSpying = false;
            yield return new WaitForSeconds(1.0f);
            float timer = 0f;
            while (timer < 2)
            {
                cubeRenderer.material.SetColor("_Color", Color.red);
                isSpying = true;
                timer = timer + Time.deltaTime;
                yield return null;
            }

            cubeRenderer.material.SetColor("_Color", Color.blue);
            isSpying = false;
        }
        
    }

    IEnumerator MakeSmall()
    {
        float timer = 0;

        while (true) // this could also be a condition indicating "alive or dead"
        {
            // we scale all axis, so they will have the same value, 
            // so we can work with a float instead of comparing vectors
            while (minSize < transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(0.3f, 0.3f, 0.3f) * Time.deltaTime * growFactor;
                yield return null;
            }
            // reset the timer

            yield return new WaitForSeconds(waitTime);

            timer = 0;
            while (1 > transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(0.3f, 0.3f, 0.3f) * Time.deltaTime * growFactor;
                yield return null;
            }

            timer = 0;
            yield return new WaitForSeconds(waitTime);
        }
    }

}
