using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRayEffect : MonoBehaviour
{

    bool xRayActive = false;
    private Dictionary<GameObject, Material> originalGearsMaterials = new Dictionary<GameObject, Material>();

    public Material transparentMaterial;
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
                    ApplyGlowingBlueEffect(materialVariator);
                }
                else
                {
                    RestoreOriginalMaterials(materialVariator);
                    RemoveGlowingBlueEffect(materialVariator);
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
                //Material material = new Material(skinnedMeshRenderer.sharedMaterial);
                Material material = new Material(transparentMaterial);
                material.mainTexture = skinnedMeshRenderer.material.mainTexture;

                //SetMaterialToTransparent(material);

                Color currentColor = material.color;
                Color transparentColor = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);
                material.color = transparentColor;

                skinnedMeshRenderer.material = material;
            }
            
        }
    }

    void ApplyGlowingBlueEffect(BirdMaterialVariator materialVariator)
    {
        // Apply glowing blue effect to the antenna
        ApplyGlowToParts(materialVariator.antenna, Color.cyan);

        // Apply glowing effect to gears, handling the default material issue
        foreach (GameObject gear in materialVariator.gears)
        {
            SkinnedMeshRenderer renderer = gear.GetComponent<SkinnedMeshRenderer>();
            if (renderer != null)
            {
                if (renderer.material.name == "Default-Material" || renderer.material.name.Contains("Default"))
                {
                    Debug.Log("Replacing Default-Material with a custom emissive material for gears.");

                    // Create a new material with the Standard shader
                    Material newGlowingMaterial = new Material(Shader.Find("Standard"));

                    // Enable emission
                    newGlowingMaterial.EnableKeyword("_EMISSION");
                    newGlowingMaterial.SetColor("_EmissionColor", new Color(0.2f, 0.7f, 1f) * 5.0f); // Bright blue glow
                    newGlowingMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

                    // Assign the new glowing material to the gears
                    renderer.material = newGlowingMaterial;
                }
                else
                {
                    // If the material is not Default-Material, modify its emission
                    Material glowingMaterial = new Material(renderer.material);
                    glowingMaterial.EnableKeyword("_EMISSION");
                    glowingMaterial.SetColor("_EmissionColor", new Color(0.2f, 0.7f, 1f) * 5.0f);
                    glowingMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                    renderer.material = glowingMaterial;
                }
            }
        }
    }

    void RemoveGlowingBlueEffect(BirdMaterialVariator materialVariator)
    {
        // Restore original materials for antenna
        RestoreOriginalMaterialsForParts(materialVariator.antenna);

        // Restore original materials for gears
        foreach (GameObject gear in materialVariator.gears)
        {
            SkinnedMeshRenderer renderer = gear.GetComponent<SkinnedMeshRenderer>();
            if (renderer != null && originalGearsMaterials.ContainsKey(gear))
            {
                renderer.material = originalGearsMaterials[gear]; // Restore original material
            }
        }
    }

    void ApplyGlowToParts(GameObject[] parts, Color glowColor)
    {
        foreach (GameObject part in parts)
        {
            SkinnedMeshRenderer renderer = part.GetComponent<SkinnedMeshRenderer>();
            if (renderer != null)
            {
                Material material = new Material(renderer.material);
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", glowColor * 2.5f); // Amplify the glow
                renderer.material = material;
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
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        Color currentColor = material.color;
        material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f); // 50% transparent
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
