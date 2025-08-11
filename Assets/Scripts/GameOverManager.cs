using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VideoPair
{
    public GameObject video;
    public GameObject rawImage;
}

public class GameOverManager : MonoBehaviour
{
    [SerializeField] VideoPair[] videoPairs;

    // Start is called before the first frame update
    void Start()
    {
        int index = ScenesController.gameOverReasonIndex;
        if (index >= 0 && index < videoPairs.Length)
        {
            videoPairs[index].video.SetActive(true);
            videoPairs[index].rawImage.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
