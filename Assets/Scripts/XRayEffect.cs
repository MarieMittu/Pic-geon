using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRayEffect : MonoBehaviour
{

    bool xRayActive = false;

    public void ActivateXRay(bool enabled)
    {
        GameObject[] roboBirds = GameObject.FindGameObjectsWithTag("RobotBird");
        GameObject[] realBirds = GameObject.FindGameObjectsWithTag("RealBird");

        GameObject[] allBirds = new GameObject[roboBirds.Length + realBirds.Length];
        roboBirds.CopyTo(allBirds, 0);
        realBirds.CopyTo(allBirds, roboBirds.Length);

        
        xRayActive = enabled;

        foreach (GameObject bird in allBirds)
        {
            BirdMaterialVariator materialVariator = bird.GetComponent<BirdMaterialVariator>();
            if (materialVariator != null)
            {
                if (xRayActive)
                {
                    ApplyTransparencyToBirdParts(materialVariator);
                    ApplyLightBlueHue(materialVariator);
                }
                else
                {
                    RestoreOriginalMaterials(materialVariator);
                    RemoveLightBlueHue(materialVariator);
                }
            }
        }
    }

    void ApplyTransparencyToBirdParts(BirdMaterialVariator materialVariator)
    {
        ApplyTransparencyToParts(materialVariator.bodyParts);
        ApplyTransparencyToParts(materialVariator.beakParts);
        ApplyTransparencyToParts(materialVariator.wingParts);
        ApplyTransparencyToParts(materialVariator.feetParts);
        ApplyTransparencyToParts(materialVariator.eyesParts);
        ApplyTransparencyToParts(materialVariator.lidsParts);
        ApplyTransparencyToParts(materialVariator.thighParts);
    }

    void ApplyTransparencyToParts(GameObject[] parts)
    {
        foreach (GameObject part in parts)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = part.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                Material material = new Material(skinnedMeshRenderer.material);

                SetMaterialToTransparent(material);

                Color currentColor = material.color;
                Color transparentColor = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);
                material.color = transparentColor;

                skinnedMeshRenderer.material = material;
            }
            
        }
    }

    void ApplyLightBlueHue(BirdMaterialVariator materialVariator)
    {
        ApplyColorToParts(materialVariator.antenna, new Color(0.5f, 0.8f, 1f)); // Light blue
        ApplyColorToParts(materialVariator.gears, new Color(0.5f, 0.8f, 1f));   // Light blue
    }

    void RemoveLightBlueHue(BirdMaterialVariator materialVariator)
    {
        RestoreOriginalMaterialsForParts(materialVariator.antenna);
        RestoreOriginalMaterialsForParts(materialVariator.gears);
    }

    void ApplyColorToParts(GameObject[] parts, Color color)
    {
        foreach (GameObject part in parts)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = part.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                Material material = new Material(skinnedMeshRenderer.material);
                material.color = color;
                skinnedMeshRenderer.material = material;
            }
        }
    }

    void RestoreOriginalMaterialsForParts(GameObject[] parts)
    {
        foreach (GameObject part in parts)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = part.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                int variation = part.GetComponentInParent<BirdMaterialVariator>().variation;
                int slotIndex = FindSlotIndex(part);
                if (slotIndex >= 0)
                {
                    Material originalMaterial = BirdMaterialVariator.materialCache[variation][slotIndex];
                    if (originalMaterial != null)
                    {
                        skinnedMeshRenderer.material = originalMaterial;
                    }
                }
            }
        }
    }

    void SetMaterialToTransparent(Material material)
    {
        material.SetFloat("_Mode", 3); // 3 corresponds to Transparent mode
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    void RestoreOriginalMaterials(BirdMaterialVariator materialVariator)
    {
        for (int i = 0; i < materialVariator.slots.Length; i++)
        {
            GameObject[] parts = materialVariator.slots[i];
            for (int j = 0; j < parts.Length; j++)
            {
                SkinnedMeshRenderer renderer = parts[j].GetComponent<SkinnedMeshRenderer>();
                Material originalMaterial = BirdMaterialVariator.materialCache[materialVariator.variation][i];
                if (renderer != null && originalMaterial != null)
                {
                    renderer.material = originalMaterial;
                }
            }
        }
    }

    int FindSlotIndex(GameObject part)
    {
        BirdMaterialVariator materialVariator = part.GetComponentInParent<BirdMaterialVariator>();
        if (materialVariator == null) return -1;

        for (int i = 0; i < materialVariator.slots.Length; i++)
        {
            if (System.Array.Exists(materialVariator.slots[i], element => element == part))
            {
                return i;
            }
        }
        return -1;
    }

    public bool IsXRayActive() { return xRayActive; }
}
