using System;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class ApplyImageEffectScript : MonoBehaviour
{

    public Material[] materials;
    bool[] enabledMaterials;
    public float startOfGlitch = 0.75f;
    bool thermalActive = false;
    bool xrayActive = false;

    [HideInInspector] public Material thermalMat;
    int thermalMatIndex;
    [HideInInspector] public Material xrayMat;
    int xrayMatIndex;
    [HideInInspector] public Material dofMat;
    int dofMatIndex;
    [HideInInspector] public Material glitchMat;
    int glitchMatIndex;

    void Start()
    {
        enabledMaterials = Enumerable.Repeat(true, materials.Length).ToArray();
        // copy materials to avoid changing the materials in the project files when parameters are changed
        if (Application.isPlaying)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                var m = materials[i];
                Material mat = new Material(m);
                materials[i] = mat;
            }
        }

        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals;

        for (int i = 0; i < materials.Length; i++)
        {
            if (null == materials[i] || null == materials[i].shader ||
               !materials[i].shader.isSupported)
            {
                enabled = false;
                return;
            }
        }

        for (int i = 0; i < materials.Length; i++)
        {
            var mat = materials[i];
            switch (mat.name)
            {
                case "DepthOfFieldShaderMaterial":
                    dofMat = mat;
                    dofMatIndex = i;
                    break;
                case "GlitchEffectShaderMaterial":
                    glitchMat = mat;
                    glitchMatIndex = i;
                    break;
                case "ThermalVisionEffectShaderMaterial":
                    thermalMat = mat;
                    thermalMatIndex = i;
                    enabledMaterials[thermalMatIndex] = thermalActive;
                    break;
                case "XrayVisionEffectShaderMaterial":
                    xrayMat = mat;
                    xrayMatIndex = i;
                    enabledMaterials[xrayMatIndex] = xrayActive;
                    break;
            }
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture[] intermediate = {
            RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format),
            RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format)
        };

        if (enabledMaterials[0]) Graphics.Blit(source, intermediate[1], materials[0]);
        else Graphics.Blit(source, intermediate[1]);
        for (int i = 1; i < materials.Length-1; i++)
        {
            if (enabledMaterials[i]) Graphics.Blit(intermediate[i % 2], intermediate[(i + 1) % 2], materials[i]);
            else Graphics.Blit(intermediate[i % 2], intermediate[(i + 1) % 2]);
        }
        Graphics.Blit(intermediate[(materials.Length-1) % 2], destination, materials[materials.Length-1]);


        RenderTexture.ReleaseTemporary(intermediate[0]);
        RenderTexture.ReleaseTemporary(intermediate[1]);
    }
    private void Update()
    {
        if (Application.isPlaying)
        {
            // update glitch effect intensity so it gradually gets stronger towards the end of the mission
            float normalizedMissionTime = 1 - GameManager.sharedInstance.missionDuration / GameManager.sharedInstance.startMissionDuration;
            float glitchIntensity = Math.Clamp(normalizedMissionTime - startOfGlitch, 0, 1) / (1 - startOfGlitch);
            //// make glitch more noticeable once it starts and give some time at full intensity
            //if (glitchIntensity > 0) glitchIntensity = Math.Clamp(glitchIntensity + 0.2f, 0, 1);
            glitchMat.SetFloat("_Intensity", glitchIntensity);

            // toggle thermal vision
            if(Input.GetKeyDown(KeyCode.T))
            {
                thermalActive = !thermalActive;
                if (thermalActive)
                {
                    for (int i = 0; i < BirdMaterialVariator.materialCache.Length; i++)
                    {
                        for (int j = 0; j < BirdMaterialVariator.materialCache[i].Length; j++)
                        {
                            Material mat = BirdMaterialVariator.materialCache[i][j];
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", Color.magenta);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < BirdMaterialVariator.materialCache.Length; i++)
                    {
                        for (int j = 0; j < BirdMaterialVariator.materialCache[i].Length; j++)
                        {
                            Material mat = BirdMaterialVariator.materialCache[i][j];
                            mat.DisableKeyword("_EMISSION");
                        }
                    }
                }
                enabledMaterials[thermalMatIndex] = thermalActive;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                xrayActive = !xrayActive;
                if (xrayActive)
                {
                    for (int i = 0; i < BirdMaterialVariator.materialCache.Length; i++)
                    {
                        for (int j = 0; j < BirdMaterialVariator.materialCache[i].Length; j++)
                        {
                            Material mat = BirdMaterialVariator.materialCache[i][j];

                            // Example of enabling X-ray effect
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", Color.cyan); // Set emission to a cyan color for X-ray
                            mat.SetFloat("_Alpha", 0.5f); // Adjust transparency for X-ray effect (requires shader support)
                        }
                    }
                } else
                {
                    for (int i = 0; i < BirdMaterialVariator.materialCache.Length; i++)
                    {
                        for (int j = 0; j < BirdMaterialVariator.materialCache[i].Length; j++)
                        {
                            Material mat = BirdMaterialVariator.materialCache[i][j];

                            // Restore original material settings
                            mat.DisableKeyword("_EMISSION");
                            mat.SetFloat("_Alpha", 1f); // Restore full opacity
                        }
                    }
                }
                enabledMaterials[xrayMatIndex] = xrayActive;
            }
        }
    }
}