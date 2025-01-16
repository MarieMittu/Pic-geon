using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectablePhoto : MonoBehaviour
{
    private Outline outline;
    public Texture2D photoTexture;
    private Image photoImage;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }
        outline.enabled = false;

        photoImage = GetComponent<Image>();
    }

    public void SelectPhoto()
    {
        if (outline != null)
        {
            outline.enabled = true; 
        }
    }
    

    public void DeselectPhoto()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public Texture2D GetPhotoTexture()
    {
        return photoTexture;
    }
}
