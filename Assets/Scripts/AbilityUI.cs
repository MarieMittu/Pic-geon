using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    Image[] buttons;
    Image[] timers;
    Text[] texts;
    string[] abilityNames = {
        "Thermal Vision",
        "Xray Vision",
        "Night Vision"
    };

    int activatedAbility = -1;

    // Start is called before the first frame update
    void Start()
    {
        var a1 = transform.GetChild(0);
        var a2 = transform.GetChild(1);
        var a3 = transform.GetChild(2);
        buttons = new Image[] {
            a1.GetChild(0).GetComponent<Image>(),
            a2.GetChild(0).GetComponent<Image>(),
            a3.GetChild(0).GetComponent<Image>(),
        };
        timers = new Image[] {
            a1.GetChild(0).GetChild(0).GetComponent<Image>(),
            a2.GetChild(0).GetChild(0).GetComponent<Image>(),
            a3.GetChild(0).GetChild(0).GetComponent<Image>(),
        };
        texts = new Text[] {
            a1.GetChild(1).GetComponent<Text>(),
            a2.GetChild(1).GetComponent<Text>(),
            a3.GetChild(1).GetComponent<Text>(),
        };
        foreach (var t in texts)
        {
            t.enabled = false;
        }
        foreach (var b in buttons)
        {
            b.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (activatedAbility != -1)
        {
            EnableAbility(abilityNames[activatedAbility]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAbilityFillAmount(string name, float amount)
    {
        int index = GetIndex(name);
        timers[index].fillAmount = amount;
    }
    public void EnableAbility(string name)
    {
        int index = GetIndex(name);
        activatedAbility = index;
        try
        {
            buttons[index].color = Color.white;
            timers[index].color = Color.white;
            timers[index].fillAmount = 0;
            buttons[index].transform.GetChild(1).gameObject.SetActive(true);
        }
        catch
        {
            
        }
    }
    public void EnableAbilityText(string name, bool enable)
    {
        int index = GetIndex(name);
        texts[index].enabled = enable;
    }

    int GetIndex(string name)
    {
        for (int i = 0; i < abilityNames.Length; i++)
        {
            if (name ==  abilityNames[i]) return i;
        }
        return -1;
    }
}
