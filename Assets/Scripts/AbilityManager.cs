using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public ApplyImageEffectScript imageEffectScript;

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
        abilities = new Ability[] {
            new Ability("Thermal Vision", KeyCode.T, 10, 0.3f, imageEffectScript.SetThermalVision)
        };
    }

    void Update()
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            var ability = abilities[i];
            if (Input.GetKeyDown(ability.key))
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

        remainingAbilityTime -= Time.deltaTime;

        if (remainingAbilityTime <= 0)
        {
            deactivateActiveAbility();
        }
    }

    void deactivateActiveAbility()
    {
        if (activeAbilityIndex == -1) return;
        abilities[activeAbilityIndex].setActive(false);
        activeAbilityIndex = -1;
    }

    void activateAbility(int index)
    {
        abilities[index].setActive(true);
        activeAbilityIndex = index;
    }
}
