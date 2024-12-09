using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LimitedCamera : MonoBehaviour
{
    public float mouseSensitivity = 2f;
    float cameraVerticalRotation = 0f;
    float cameraHorizontalRotation = 0f;

    float referenceHorizontalRotation;
    float referenceVerticalRotation;

    // relative to reference rotation
    [Range(-0.1f, -90f)] public float minHorizontalRotation;
    [Range(0.1f, 90f)] public float maxHorizontalRotation;
    [Range(-0.1f, -90f)] public float minVerticalRotation;
    [Range(0.1f, 90f)] public float maxVerticalRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraHorizontalRotation = transform.localEulerAngles.y;
        cameraVerticalRotation = transform.localEulerAngles.x;
        referenceHorizontalRotation = cameraHorizontalRotation;
        referenceVerticalRotation = cameraVerticalRotation;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        //absolute values
        float minHRot = referenceHorizontalRotation + minHorizontalRotation;
        float maxHRot = referenceHorizontalRotation + maxHorizontalRotation;
        float minVRot = referenceVerticalRotation + minVerticalRotation;
        float maxVRot = referenceVerticalRotation + maxVerticalRotation;

        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, minVRot, maxVRot);
        cameraHorizontalRotation += inputX;
        cameraHorizontalRotation = Mathf.Clamp(cameraHorizontalRotation, minHRot, maxHRot);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation + Vector3.up * cameraHorizontalRotation;
    }

    private void OnDrawGizmosSelected()
    {
        // absolute values
        float minHRot = transform.localEulerAngles.y + minHorizontalRotation;
        float maxHRot = transform.localEulerAngles.y + maxHorizontalRotation;
        float minVRot = transform.localEulerAngles.x + minVerticalRotation;
        float maxVRot = transform.localEulerAngles.x + maxVerticalRotation;

        float fovVert = maxVRot - minVRot;
        float fovHori = maxHRot - minHRot;

        // add the camera's fov, so we can see the actual edges of what can be seen
        Camera cam = GetComponent<Camera>();
        fovVert += cam.fieldOfView;
        fovHori += cam.fieldOfView * cam.aspect;

        float aspect = fovHori / fovVert;

        float centralDirV = (maxVRot + minVRot) / 2;
        float centralDirH = (maxHRot + minHRot) / 2;
        float dirDiffV = centralDirV - referenceVerticalRotation;
        float dirDiffH = centralDirH - referenceHorizontalRotation;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(centralDirV, centralDirH, 0), transform.localScale);
        Gizmos.color = Color.red;
        Gizmos.DrawFrustum(Vector3.zero, fovVert, 100, 1, aspect);
    }
}
