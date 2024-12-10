using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdsSpawner : MonoBehaviour
{
    public Transform[] birdsPositions;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBirds();
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
}
