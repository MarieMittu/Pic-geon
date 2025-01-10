using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class ApplyImageEffectScript : MonoBehaviour
{

    public Material[] materials;

    void Start()
    {
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
}