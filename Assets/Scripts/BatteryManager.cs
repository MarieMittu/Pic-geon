using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryManager : MonoBehaviour
{
    public static BatteryManager instance;

    float charge;
    public float maxCharge;
    public BatteryUI batteryUI;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        charge = maxCharge;
        updateBatteryUI();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Decreases the remaining charge by an amount if enough charge is left.
    /// </summary>
    /// <param name="amount">the amount of charge you try to use</param>
    /// <param name="useInsufficientCharge">whether to use decrease the remaining charge at all if it is lower than amount</param>
    /// <returns>
    /// the charge you actually used
    /// </returns>
    public float useCharge(float amount, bool useInsufficientCharge = true)
    {
        float usedCharge;
        if (charge < amount)
        {
            if (useInsufficientCharge)
            {
                usedCharge = charge;
                charge = 0;
            }
            else
            {
                usedCharge = 0;
            }
        }
        else
        {
            charge -= amount;
            usedCharge = amount;
        }
        updateBatteryUI();
        if (charge <= 0)
        {
            GameManager.sharedInstance.TriggerGameOver();
        }
        return usedCharge;
    }

    public void refillCharge()
    {
        charge = maxCharge;
    }

    void updateBatteryUI()
    {
        batteryUI.SetBatteryLevel((int)charge);
    }
}
