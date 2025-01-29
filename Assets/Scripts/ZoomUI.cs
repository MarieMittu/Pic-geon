using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomUI : MonoBehaviour
{
    public void SetZoomRatios(float[] fovs)
    {
        transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "x"+Mathf.RoundToInt(fovs[0] / fovs[1]);
        transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "x"+Mathf.RoundToInt(fovs[0] / fovs[2]);
    }

    public void SetZoomLevel(int index)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetChild(1).gameObject.SetActive(i == index);
        }
    }
}
