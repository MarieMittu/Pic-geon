using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdsSpawner : MonoBehaviour
{
    public Transform[] birdsPositions;
    public Transform[] robotsPositions;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBirds();
        SpawnRobots();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void SpawnBirds()
    {
       

        for (int i = 0; i < birdsPositions.Length; i++)
        {
            GameObject selectedBird = ObjectPool.SharedInstance.GetPooledBird();
            selectedBird.transform.position = birdsPositions[i].position;
            selectedBird.transform.rotation = birdsPositions[i].rotation;

            selectedBird.SetActive(true);
            Debug.Log("num of birds");
        }
        
    }

    void SpawnRobots()
    {


        for (int i = 0; i < robotsPositions.Length; i++)
        {
            GameObject selectedRobot = ObjectPool.SharedInstance.GetPooledRobot();
            selectedRobot.transform.position = robotsPositions[i].position;
            selectedRobot.transform.rotation = robotsPositions[i].rotation;

            selectedRobot.SetActive(true);
        }

    }
}
