using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

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

    //[Range(0.0f, 90f)] public float minFOV = 10.0f;
    //[Range(0.0f, 90f)] public float maxFOV = 60.0f;
    [Range(0, 90)] public float[] zoomLevels = { 60, 30, 10 };
    [Range(0, 90)] public float[] zoomLevelsFocalLength = { 40, 60, 80 };
    public float minFocusDistance = 0;
    public float maxFocusDistance = 3;
    int currentZoomLevel = 0;
    public bool isScrollEnabled = true;

    public int tapeLimit;
    private int originalTapeLimit;
    private int usedTape = 0;
    private int correctPhotosAmount = 0;
    public Text tapeText;

    // Start is called before the first frame update
    void Start()
    {
        cameraHorizontalRotation = transform.localEulerAngles.y;
        cameraVerticalRotation = transform.localEulerAngles.x;
        referenceHorizontalRotation = cameraHorizontalRotation;
        referenceVerticalRotation = cameraVerticalRotation;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        originalTapeLimit = tapeLimit;
        tapeText.text = usedTape + "/" + tapeLimit + " tape used";
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        ProcessCameraMovement(inputX, inputY);

        if (isScrollEnabled)
        {
            DepthOfField dph = GetDepthOfField();
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                Camera cam = GetComponent<Camera>();
                currentZoomLevel += Math.Sign(Input.mouseScrollDelta.y);
                currentZoomLevel = Math.Clamp(currentZoomLevel, 0, zoomLevels.Length - 1);
                cam.fieldOfView = zoomLevels[currentZoomLevel];
                //cam.fieldOfView -= Input.mouseScrollDelta.y;
                //cam.fieldOfView = Math.Clamp(cam.fieldOfView, minFOV, maxFOV);
                dph.focalLength.value = zoomLevelsFocalLength[currentZoomLevel];
            }

            int focalDepthDirection = 0;
            if (Input.GetKey(KeyCode.W)) focalDepthDirection += 1;
            if (Input.GetKey(KeyCode.S)) focalDepthDirection -= 1;
            dph.focusDistance.value += focalDepthDirection * (zoomLevels[currentZoomLevel] / 100f) * 1.0f * Time.deltaTime;
        }
        

        // photo
        if (!GameManager.sharedInstance.isGamePaused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DetectBirdsOnPhoto();
                TrackTapeAmount();
            }
        }
        
        TrackTime();

        if (GameManager.sharedInstance.isGamePaused)
        {
            mouseSensitivity = 0;
            isScrollEnabled = false;
        } else
        {
            mouseSensitivity = 2f;
            isScrollEnabled = true;
        }
    }

    private DepthOfField GetDepthOfField()
    {
        List<PostProcessVolume> volList = new List<PostProcessVolume>();
        PostProcessManager.instance.GetActiveVolumes(GetComponent<PostProcessLayer>(), volList, true, true);
        DepthOfField dph;
        foreach (PostProcessVolume vol in volList)
        {
            PostProcessProfile ppp = vol.profile;
            if (ppp)
            {
                ppp.TryGetSettings<DepthOfField>(out dph);
                return dph;
            }
        }
        return null;
    }

    void ProcessCameraMovement(float inputX, float inputY)
    {
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

    void DetectBirdsOnPhoto()
    {
        GameObject[] roboBirds = GameObject.FindGameObjectsWithTag("RobotBird");
        foreach (GameObject rb in roboBirds)
        {
            var robotScript = rb.GetComponent<AIRobotController>();
            if (rb.GetComponent<MeshRenderer>().isVisible && robotScript.isSpying)
            {
                Ray ray = new Ray(transform.position, rb.transform.position - transform.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "RobotBird")
                {
                    correctPhotosAmount++;
                    Debug.Log("sus bird on photo " + correctPhotosAmount);  
                }
                else Debug.Log("sus bird obstructed ");
            }
            else if (rb.GetComponent<MeshRenderer>().isVisible && !robotScript.isSpying)
            {
                Debug.Log("wrong timing");
            }
        }
    }

    void TrackTapeAmount()
    {
        tapeLimit--;
        usedTape++;
        tapeText.text = usedTape + "/" + originalTapeLimit + " tape used";
        Debug.Log("TapeLimit current " + tapeLimit);

        if (tapeLimit <= 0)
        {
            ControlCorrectPhotos();
        }
    }

    void TrackTime()
    {
        if (GameManager.sharedInstance.missionDuration <= 0)
        {
            ControlCorrectPhotos();
        }
    }

    void ControlCorrectPhotos()
    {
        if (correctPhotosAmount > 0)
        {
            GameManager.sharedInstance.TriggerNextLevel();
        }
        else
        {
            GameManager.sharedInstance.TriggerGameOver();
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
