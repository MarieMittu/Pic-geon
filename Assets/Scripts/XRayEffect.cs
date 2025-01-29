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
                }
                else
                {
                    RestoreOriginalMaterials(materialVariator);
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

    public bool IsXRayActive() { return xRayActive; }
}
