using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool SharedInstance;

    public List<GameObject> pooledBirds; //birds who appeared in the scene already
    public GameObject[] birdsToPool; //array of various birds prefabs
    public int birdsAmountToPool; //a number big enough to not have a shortage of birds, e.g. 20

    public List<GameObject> pooledRobots; //birds who appeared in the scene already
    public GameObject[] robotsToPool; //array of various birds prefabs
    public int robotsAmountToPool; //a number big enough to not have a shortage of birds, e.g. 20

    void Awake()
    {
        SharedInstance = this;

        pooledBirds = new List<GameObject>();
        for (int i = 0; i < birdsAmountToPool; i++)
        {
            int randomIndex = Random.Range(0, birdsToPool.Length);
            GameObject tmpBirds = Instantiate(birdsToPool[randomIndex]);
            tmpBirds.SetActive(false);
            pooledBirds.Add(tmpBirds);
        }

        pooledRobots = new List<GameObject>();
        for (int i = 0; i < robotsAmountToPool; i++)
        {
            int randomIndex = Random.Range(0, robotsToPool.Length);
            GameObject tmpBirds = Instantiate(robotsToPool[randomIndex]);
            tmpBirds.SetActive(false);
            pooledRobots.Add(tmpBirds);
        }
    }

    public GameObject GetPooledBird()
    {
        for (int i = 0; i < pooledBirds.Count; i++)
        {
            if (!pooledBirds[i].activeInHierarchy)
            {
                return pooledBirds[i];
            }
        }
        return null;
    }

    public GameObject GetPooledRobot()
    {
        for (int i = 0; i < pooledRobots.Count; i++)
        {
            if (!pooledRobots[i].activeInHierarchy)
            {
                return pooledRobots[i];
            }
        }
        return null;
    }
}
