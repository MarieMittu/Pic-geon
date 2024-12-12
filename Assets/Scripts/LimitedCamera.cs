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
    [Range(-0.0f, -90f)] public float minHorizontalRotation;
    [Range(0.0f, 90f)] public float maxHorizontalRotation;
    [Range(-0.0f, -90f)] public float minVerticalRotation;
    [Range(0.0f, 90f)] public float maxVerticalRotation;
    
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

        if (Input.mouseScrollDelta != Vector2.zero)
        {
            GetComponent<Camera>().fieldOfView -= Input.mouseScrollDelta.y;
        }

        // photo
        if (Input.GetMouseButtonDown(0))
        {
            GameObject[] roboBirds = GameObject.FindGameObjectsWithTag("RobotBird");
            foreach (GameObject rb in roboBirds)
            {
                var robotScript = rb.GetComponent<AIRobotController>();
                if (rb.GetComponent<MeshRenderer>().isVisible && robotScript.isSpying) //add check if performing sus action
                {
                    Debug.Log("sus bird on photo");
                } else if (rb.GetComponent<MeshRenderer>().isVisible && !robotScript.isSpying)
                {
                    Debug.Log("wrong timing");
                }
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        // absolute values
        float minHRot = (Application.isPlaying ? referenceHorizontalRotation : transform.localEulerAngles.y) + minHorizontalRotation;
        float maxHRot = (Application.isPlaying ? referenceHorizontalRotation : transform.localEulerAngles.y) + maxHorizontalRotation;
        float minVRot = (Application.isPlaying ? referenceVerticalRotation : transform.localEulerAngles.x) + minVerticalRotation;
        float maxVRot = (Application.isPlaying ? referenceVerticalRotation : transform.localEulerAngles.x) + maxVerticalRotation;

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

        Gizmos.color = Color.red;
        Vector3 vLU = Quaternion.Euler(new Vector3(minVRot - cam.fieldOfView/2, minHRot - cam.aspect*cam.fieldOfView*0.5f, 0)) * Vector3.forward;
        Gizmos.DrawLine(transform.position, transform.position + vLU * 1000);
        Vector3 vLD = Quaternion.Euler(new Vector3(maxVRot + cam.fieldOfView / 2, minHRot - cam.aspect * cam.fieldOfView * 0.5f, 0)) * Vector3.forward;
        Gizmos.DrawLine(transform.position, transform.position + vLD * 1000);
        Vector3 vRU = Quaternion.Euler(new Vector3(minVRot - cam.fieldOfView / 2, maxHRot + cam.aspect * cam.fieldOfView * 0.5f, 0)) * Vector3.forward;
        Gizmos.DrawLine(transform.position, transform.position + vRU * 1000);
        Vector3 vRD = Quaternion.Euler(new Vector3(maxVRot + cam.fieldOfView / 2, maxHRot + cam.aspect * cam.fieldOfView * 0.5f, 0)) * Vector3.forward;
        Gizmos.DrawLine(transform.position, transform.position + vRD * 1000);

        //Gizmos.DrawLine(transform.position + vRD * 10, transform.position + vRU * 10);
        //Gizmos.DrawLine(transform.position + vLD * 10, transform.position + vLU * 10);
        //Gizmos.DrawLine(transform.position + vRD * 10, transform.position + vLD * 10);
        //Gizmos.DrawLine(transform.position + vLU *10, transform.position + vRU *10);

        //Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(centralDirV, centralDirH, 0), transform.localScale);
        //Gizmos.DrawFrustum(Vector3.zero, fovVert, 100, 1, aspect);
    }
}
