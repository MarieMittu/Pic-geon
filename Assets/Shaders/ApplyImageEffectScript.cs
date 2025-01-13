using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class ApplyImageEffectScript : MonoBehaviour
{

    public Material[] materials;
    public float startOfGlitch = 0.75f;

    void Start()
    {
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
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;

        for (int i = 0; i < materials.Length; i++)
        {
            if (null == materials[i] || null == materials[i].shader ||
               !materials[i].shader.isSupported)
            {
                enabled = false;
                return;
            }
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture[] intermediate = {
            RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format),
            RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format)
        };

        Graphics.Blit(source, intermediate[1], materials[0]);
        for (int i = 1; i < materials.Length-1; i++)
        {
            Graphics.Blit(intermediate[i%2], intermediate[(i+1)%2], materials[i]);
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
            Material glitchMat = materials[1];
            float normalizedMissionTime = 1 - GameManager.sharedInstance.missionDuration / GameManager.sharedInstance.startMissionDuration;
            float glitchIntensity = Math.Clamp(normalizedMissionTime - startOfGlitch, 0, 1) / (1 - startOfGlitch);
            //// make glitch more noticeable once it starts and give some time at full intensity
            //if (glitchIntensity > 0) glitchIntensity = Math.Clamp(glitchIntensity + 0.2f, 0, 1);
            glitchMat.SetFloat("_Intensity", glitchIntensity);
        }
    }
}