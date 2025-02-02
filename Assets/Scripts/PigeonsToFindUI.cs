using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonsToFindUI : MonoBehaviour
{
    public GameObject birdIcon;

    GameObject[] birdIcons;

    int numBirds = 0;
    int numFound = 0;

    public void Start()
    {
        if (MissionManager.sharedInstance.isTutorial) return;
        numBirds = MissionManager.sharedInstance.GetRequiredPhotos();
        float spacing = 40;
        for (int i = 0; i < numBirds; i++)
        {
            var icon = Instantiate(birdIcon, transform);
            icon.transform.localPosition = new Vector3( (1-numBirds) * spacing/2.0f + i * spacing, 0, 0);
        }
        SetFound(numFound);
    }

    public void SetFound(int numberFound)
    {
        if (numberFound > numFound) GetComponent<AudioSource>().Play();
        numFound = numberFound;
        for (int i = 0; i < numBirds; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(i >= numberFound);
            transform.GetChild(i).GetChild(1).gameObject.SetActive(i < numberFound);
        }
    }
}
