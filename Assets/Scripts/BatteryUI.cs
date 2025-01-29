using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryUI : MonoBehaviour
{
    public int batteryLevel = 0;

    Transform emptySlots;
    Transform halfsSlots;
    Transform fullSlots;

    // Start is called before the first frame update
    void Start()
    {
        emptySlots = transform.Find("EmptySlots");
        halfsSlots = transform.Find("HalfSlots");
        fullSlots = transform.Find("FullSlots");
    }

    public void SetBatteryLevel(int level)
    {
        if (emptySlots == null)
        {
            emptySlots = transform.Find("EmptySlots");
            halfsSlots = transform.Find("HalfSlots");
            fullSlots = transform.Find("FullSlots");
        }
        batteryLevel = level;
        for (int i = 0; i < emptySlots.childCount; i++)
        {
            var slotE = emptySlots.GetChild(i);
            var slotH = halfsSlots.GetChild(i);
            var slotF = fullSlots.GetChild(i);
            if (i > batteryLevel/2)
            {
                slotE.gameObject.SetActive(true);
                slotH.gameObject.SetActive(false);
                slotF.gameObject.SetActive(false);
            }
            else if (i < batteryLevel/2)
            {
                slotE.gameObject.SetActive(false);
                slotH.gameObject.SetActive(false);
                slotF.gameObject.SetActive(true);
            }
            else
            {
                slotE.gameObject.SetActive(batteryLevel % 2 == 0);
                slotH.gameObject.SetActive(batteryLevel % 2 == 1);
                slotF.gameObject.SetActive(false);
            }
        }
    }
}
