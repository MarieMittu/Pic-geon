using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void SetFocusColor(Color color)
    {
        Debug.Log("setting color focus " + color);
        foreach (Transform child in transform)
        {
            //SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            Image image = child.GetComponent<Image>();
            image.color = color;
            //sr.color = color;
        }
    }
}
