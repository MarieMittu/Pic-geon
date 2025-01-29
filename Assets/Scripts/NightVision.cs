using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightVision : MonoBehaviour
{
    public Light[] normalLights;
    public Light[] hiddenLights;
    public GameObject nightImage;
    bool nightActive = false;

    public void ActivateNightVision(bool enabled)
    {
        nightActive = enabled;

        if (nightActive) AdjustLights(); else ReturnLights();
    }

    public void AdjustLights()
    {
        for (int i = 0; i < normalLights.Length; i++)
        {
            normalLights[i].intensity = 100;
        }

        for (int i = 0; i < hiddenLights.Length; i++)
        {
            hiddenLights[i].intensity = 0.5f;
        }
        nightImage.SetActive(true);
    }

    public void ReturnLights()
    {
        for (int i = 0; i < normalLights.Length; i++)
        {
            normalLights[i].intensity = 1;
        }

        for (int i = 0; i < hiddenLights.Length; i++)
        {
            hiddenLights[i].intensity = 0;
        }
        nightImage.SetActive(false);
    }

    public bool IsNightVisionActive() { return nightActive; }
}
