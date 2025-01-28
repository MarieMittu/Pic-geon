using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusBoxUI : MonoBehaviour
{
    public void SetFocusSize(float s)
    {
        Transform corners = transform.GetChild(0);
        Transform circle = transform.GetChild(1);
        var outerScale = s * 6f + 1;
        corners.localScale = new Vector3(1, 1, 1) * 2.5f;//Mathf.Clamp(outerScale, 1, 3);
        circle.localScale = new Vector3(1,1,1) * Mathf.Clamp(outerScale * (s * 1.75f + 1), 1, 4.5f);
    }
}
