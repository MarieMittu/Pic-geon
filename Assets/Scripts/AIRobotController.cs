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
        float colorDuration = 3f; 
        float timer = 0f;

        while (timer < colorDuration)
        {
            
                cubeRenderer.material.SetColor("_Color", Color.red);
                isSpying = true;
                timer += Time.deltaTime;
                yield return null;
            

            
        }
        cubeRenderer.material.SetColor("_Color", Color.blue);
        isSpying = false;

    }

    //IEnumerator MakeSmall()
    //{
        
    //}

}
