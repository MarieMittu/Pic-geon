using System.Collections;
using System.Collections.Generic;
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
    [Range(-180, 0)] public float minHorizontalRotation;
    [Range(0, 190)] public float maxHorizontalRotation;
    [Range(-90, 0)] public float minVerticalRotation;
    [Range(0, 90)] public float maxVerticalRotation;

    // Start is called before the first frame update
    void Start()
    {
        cameraHorizontalRotation = transform.localEulerAngles.y;
        cameraVerticalRotation = transform.localEulerAngles.x;
        referenceHorizontalRotation = cameraHorizontalRotation;
        referenceVerticalRotation = cameraVerticalRotation;

        //absolute values
        float minHRot = referenceHorizontalRotation + minHorizontalRotation;
        float maxHRot = referenceHorizontalRotation + maxHorizontalRotation;
        float minVRot = referenceVerticalRotation + minVerticalRotation;
        float maxVRot = referenceVerticalRotation + maxVerticalRotation;

        Vector3 cornerDirLU = new Vector3(Mathf.Cos(minHRot) * Mathf.Cos(minVRot), Mathf.Sin(minHRot) * Mathf.Cos(minVRot), Mathf.Sin(minVRot)) * 100;
        Debug.DrawRay(transform.position, cornerDirLU, Color.red, float.PositiveInfinity);
        Vector3 cornerDirRU = new Vector3(Mathf.Cos(maxHRot) * Mathf.Cos(minVRot), Mathf.Sin(maxHRot) * Mathf.Cos(minVRot), Mathf.Sin(minVRot)) * 100;
        Debug.DrawRay(transform.position, cornerDirRU, Color.red, float.PositiveInfinity);
        Vector3 cornerDirLD = new Vector3(Mathf.Cos(minHRot) * Mathf.Cos(maxVRot), Mathf.Sin(minHRot) * Mathf.Cos(maxVRot), Mathf.Sin(maxVRot)) * 100;
        Debug.DrawRay(transform.position, cornerDirLD, Color.red, float.PositiveInfinity);
        Vector3 cornerDirRD = new Vector3(Mathf.Cos(maxHRot) * Mathf.Cos(maxVRot), Mathf.Sin(maxHRot) * Mathf.Cos(maxVRot), Mathf.Sin(maxVRot)) * 100;
        Debug.DrawRay(transform.position, cornerDirRD, Color.red, float.PositiveInfinity);

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
}
