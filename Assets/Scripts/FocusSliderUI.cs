using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusSliderUI : MonoBehaviour
{
    Transform bar;
    public float verticalPadding = 0.3f; // fraction of sprite height to leave empty
    public float screenHeightRatio = 0.8f;
    float barMinY;
     float barMaxY;

    // Start is called before the first frame update
    void Start()
    {
        bar = transform.GetChild(0);
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float halfHeight = spriteRenderer.bounds.size.y / 2f;

            // Adjust by bar pivot (if needed)
            barMinY = -halfHeight;
            barMaxY = halfHeight;
        }
        else
        {
            // fallback for UI (RectTransform based)
            var rect = GetComponent<RectTransform>();
            float halfHeight = rect.rect.height / 2f;
            barMinY = -halfHeight;
            barMaxY = halfHeight;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLayout();
    }

    void UpdateLayout()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null || bar == null) return;

        // 1. Scale the sprite height to a fraction of screen height
        float canvasHeight = rt.root.GetComponent<RectTransform>().rect.height;
        float targetHeight = canvasHeight * screenHeightRatio;

        Vector2 size = rt.sizeDelta;
        size.y = targetHeight;
        rt.sizeDelta = size;

        // 2. Compute bar min/max with padding
        float halfHeight = rt.rect.height / 2f;
        float pad = halfHeight * verticalPadding;

        barMinY = -halfHeight + pad;
        barMaxY = halfHeight - pad;
    }

    public void SetFocusDistance(float fD, float minFocusDistance, float maxFocusDistance)
    {

        float ratio = Mathf.InverseLerp(minFocusDistance, maxFocusDistance, fD);

        bar.localPosition = new Vector3(
            0,
            Mathf.Lerp(barMinY, barMaxY, ratio),
            0
        );
    }

}
