using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class ApplyImageEffectScript : MonoBehaviour
{

    public Material material;

    void Start()
    {
        if (Application.isPlaying) {
            Material mat = new Material(material);
            material = mat;
        }

        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;

        if (null == material || null == material.shader ||
           !material.shader.isSupported)
        {
            enabled = false;
            return;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}