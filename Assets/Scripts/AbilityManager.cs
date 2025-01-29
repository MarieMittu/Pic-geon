using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class AbilityManager : MonoBehaviour
{
    public ApplyImageEffectScript imageEffectScript;
    public Image heatVisionTimer;
    public GameObject heatVisionText;

    public XRayEffect xrayEffectScript;
    public NightVision nightVisionScript;

    float remainingAbilityTime = 0;

    private class Ability
    {
        public string name;
        public KeyCode key;
        public float duration;
        public float chargeUseRate;
        public Action<bool> setActive;

        public Ability(string name, KeyCode key, float duration, float chargeUseRate, Action<bool> setActive)
        {
            this.name = name;
            this.key = key;
            this.duration = duration;
            this.chargeUseRate = chargeUseRate;
            this.setActive = setActive;
        }
    }

    Ability[] abilities;
    // -1 means no ability is active
    int activeAbilityIndex = -1;

    void Start()
    {
        if (imageEffectScript == null)
        {
            imageEffectScript = GetComponent<ApplyImageEffectScript>();
        }
        if (heatVisionText == null)
        {
            heatVisionText = heatVisionTimer.transform.GetChild(0).gameObject;
            heatVisionText.SetActive(false);
        }
        if (xrayEffectScript == null)
        {
            xrayEffectScript = GetComponent<XRayEffect>();
        }
        if ( nightVisionScript == null)
        {
            nightVisionScript = GetComponent<NightVision>();
            if (nightVisionScript != null) nightVisionScript.ReturnLights();
        }
        switch (MissionManager.sharedInstance.currentMission)
        {
            case 1: // tutorial
                abilities = new Ability[] {
                    new Ability("Thermal Vision", KeyCode.F, 10, 0.3f, imageEffectScript.SetThermalVision)
                };
                break;
            case 2: // lvl one: plaza
                abilities = new Ability[] {
                    new Ability("Thermal Vision", KeyCode.F, 10, 0.3f, imageEffectScript.SetThermalVision)
                };
                break;
            case 3: // lvl two: rooftop
                abilities = new Ability[] {
                    new Ability("Xray Vision", KeyCode.F, 10, 0.3f, xrayEffectScript.ActivateXRay)
                };
                break;
            case 4: // lvl three: train station
                abilities = new Ability[] {
                    new Ability("Night Vision", KeyCode.F, 10, 0.3f, nightVisionScript.ActivateNightVision)
                };
                break;
        }
    }

    void Update()
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            var ability = abilities[i];
            if (Input.GetKeyDown(ability.key))
            {
                if (!MissionManager.sharedInstance.isTutorial || (MissionManager.sharedInstance.isTutorial && TutorialManager.sharedInstance.currentIndex > 5))
                {
                    // if this ability is already active
                    if (activeAbilityIndex == i)
                    {
                        deactivateActiveAbility();
                        remainingAbilityTime = 0;
                    }
                    else
                    {
                        deactivateActiveAbility();
                        remainingAbilityTime = BatteryManager.instance.useCharge(ability.duration * ability.chargeUseRate) / ability.chargeUseRate;
                        if (remainingAbilityTime > 0) activateAbility(i);
                    }
                }
            }
        }

        remainingAbilityTime -= Time.deltaTime;

        if (activeAbilityIndex != -1 && abilities[activeAbilityIndex].name == "Thermal Vision")
        {
            heatVisionTimer.fillAmount = remainingAbilityTime / abilities[activeAbilityIndex].duration;
        }

        if (remainingAbilityTime <= 0)
        {
            deactivateActiveAbility();
        }
    }

    void deactivateActiveAbility()
    {
        if (activeAbilityIndex == -1) return;
        abilities[activeAbilityIndex].setActive(false);
        if (abilities[activeAbilityIndex].name == "Thermal Vision")
        {
            heatVisionTimer.fillAmount = 0;
            heatVisionText.SetActive(false);
        }
        activeAbilityIndex = -1;
    }

    void activateAbility(int index)
    {
        abilities[index].setActive(true);
        activeAbilityIndex = index;
        if (abilities[activeAbilityIndex].name == "Thermal Vision")
        {
            heatVisionTimer.fillAmount = 1;
            heatVisionText.SetActive(true);
        }
    }
}
