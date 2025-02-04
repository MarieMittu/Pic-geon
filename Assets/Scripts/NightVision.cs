using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightVision : MonoBehaviour
{
    bool nightActive = false;

    [Header("Game Objects")]
    public Light[] normalLights;
    public Light[] hiddenLights;
    public GameObject nightImage;
    public GameObject grayLevel;
    public GameObject colorLevel;

    [Header("Night Vision Light Parameters")] // when night vision is activated
    public float night_NormalIntensity = 100f; // for lamps
    public float night_HiddenIntensity = 0.5f; // for ambient light

    [Header("Standard Light Parameters")] // when night vision is deactivated
    public float standard_NormalIntensity = 1f;
    public float standard_HiddenIntensity = 0.03f;


    public void ActivateNightVision(bool enabled)
    {
        nightActive = enabled;

        if (nightActive) AdjustLights(); else ReturnLights();
    }

    public void AdjustLights()
    {
        for (int i = 0; i < normalLights.Length; i++)
        {
            normalLights[i].intensity = night_NormalIntensity;
        }

        for (int i = 0; i < hiddenLights.Length; i++)
        {
            hiddenLights[i].intensity = night_HiddenIntensity;
        }
        nightImage.SetActive(true);
        grayLevel.SetActive(true);
        colorLevel.SetActive(false);
    }

    public void ReturnLights()
    {
        for (int i = 0; i < normalLights.Length; i++)
        {
            normalLights[i].intensity = standard_NormalIntensity;
        }

        for (int i = 0; i < hiddenLights.Length; i++)
        {
            hiddenLights[i].intensity = standard_HiddenIntensity;
        }
        nightImage.SetActive(false);
        grayLevel.SetActive(false);
        colorLevel.SetActive(true);

    }

    public bool IsNightVisionActive() { return nightActive; }
}
