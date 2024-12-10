using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool SharedInstance;

    public List<GameObject> pooledBirds; //birds who appeared in the scene already
    public GameObject[] birdsToPool; //array of various birds prefabs
    public int birdsAmountToPool; //a number big enough to not have a shortage of birds, e.g. 20

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
}
