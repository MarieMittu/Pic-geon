using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdsSpawner : MonoBehaviour
{
    public Transform[] birdsPositions;
    public Transform[] robotsPositions;
    public int numberOfBirdsToSpawn = 6; 
    public int numberOfRobotsToSpawn = 3;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBirds();
        SpawnRobots();
    }

    void SpawnBirds()
    {
        List<Transform> selectedBirdPositions = GetRandomPositions(birdsPositions, numberOfBirdsToSpawn);

        foreach (Transform position in selectedBirdPositions)
        {
            GameObject selectedBird = ObjectPool.SharedInstance.GetPooledBird();
            selectedBird.transform.position = position.position;
            selectedBird.transform.rotation = position.rotation;

            selectedBird.SetActive(true);
            Debug.Log("num of birds");
        }
        
    }

    void SpawnRobots()
    {
        List<Transform> selectedRobotPositions = GetRandomPositions(robotsPositions, numberOfRobotsToSpawn);

        foreach (Transform position in selectedRobotPositions)
        {
            GameObject selectedRobot = ObjectPool.SharedInstance.GetPooledRobot();
            selectedRobot.transform.position = position.position;
            selectedRobot.transform.rotation = position.rotation;

            selectedRobot.SetActive(true);
        }

    }

    List<Transform> GetRandomPositions(Transform[] positionsArray, int count)
    {
        List<Transform> positionsList = new List<Transform>(positionsArray);
        List<Transform> selectedPositions = new List<Transform>();

        for (int i = 0; i < positionsList.Count; i++)
        {
            int randomIndex = Random.Range(i, positionsList.Count);
            (positionsList[i], positionsList[randomIndex]) = (positionsList[randomIndex], positionsList[i]);
        }

        for (int i = 0; i < Mathf.Min(count, positionsList.Count); i++)
        {
            selectedPositions.Add(positionsList[i]);
        }

        return selectedPositions;
    }
}
