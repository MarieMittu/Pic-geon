using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusSliderUI : MonoBehaviour
{
    Transform bar;
    public float barMinY = -10;
    public float barMaxY = 10;

    // Start is called before the first frame update
    void Start()
    {
        bar = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFocusDistance(float fD, float minFocusDistance, float maxFocusDistance)
    {
        var ratio = (fD-minFocusDistance) / (maxFocusDistance-minFocusDistance);

        bar.localPosition = new Vector3(0, Mathf.Lerp(barMinY, barMaxY, ratio), 0);
    }

}
