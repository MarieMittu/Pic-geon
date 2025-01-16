using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectablePhoto : MonoBehaviour
{
    private Outline outline;

    private void Awake()
    {
        // Cache the Outline component
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }
        outline.enabled = false; // Ensure it's disabled by default
    }

    // Method to highlight the photo
    public void SelectPhoto()
    {
        if (outline != null)
        {
            outline.enabled = true; // Enable outline for selection
        }
    }
    

    // Method to remove highlight from the photo
    public void DeselectPhoto()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
