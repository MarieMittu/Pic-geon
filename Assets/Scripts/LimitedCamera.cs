using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LimitedCamera : MonoBehaviour
{
    [Header("Camera Rotation")]
    //public float mouseSensitivity = 2f;
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
    [Header("Zoom and Focus")]
    [Range(0, 90)] public float[] zoomLevels = { 60, 30, 10 };
    //[Range(0, 10)]
    public float[] zoomLevelsDepthOfField = { 3, 2, 1 };
    public float[] zoomLevelsMouseSensitivity = { 3, 2, 1 };
    public float minFocusDistance = 0;
    public float maxFocusDistance = 3;
    int currentZoomLevel = 0;
    public float focusDistanceSpeed = 1f;
    public float maxPeripheryBlurRadius = 0.5f;
    float peripheryBlurRadius = 0.5f;
    bool focusMode = false;
    bool realBirdInFocus = false;

    [Header("Other")]
    private int correctPhotosAmount = 0;
    public RawImage screenshotImage;
    public RawImage flashImage;
    bool photoAnimationInProgress = false;
    public bool savePhotos = false;
    Camera cam;
    ApplyImageEffectScript effectScript;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        effectScript = GetComponent<ApplyImageEffectScript>();
        cameraHorizontalRotation = transform.localEulerAngles.y;
        cameraVerticalRotation = transform.localEulerAngles.x;
        referenceHorizontalRotation = cameraHorizontalRotation;
        referenceVerticalRotation = cameraVerticalRotation;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        cam.fieldOfView = zoomLevels[currentZoomLevel];
        peripheryBlurRadius = maxPeripheryBlurRadius;
        Material dofShaderMat = effectScript.dofMat;
        dofShaderMat.SetFloat("_FocusDistance", (minFocusDistance+maxFocusDistance)/2);
        dofShaderMat.SetFloat("_DepthOfFieldSize", zoomLevelsDepthOfField[currentZoomLevel]);
        dofShaderMat.SetFloat("_PeripheryBlurRadius", peripheryBlurRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.sharedInstance.isGamePaused && !photoAnimationInProgress)
        {
            Material dofShaderMat = effectScript.dofMat;
            if (!focusMode)
            {
                float inputX = Input.GetAxis("Mouse X") * zoomLevelsMouseSensitivity[currentZoomLevel];
                float inputY = Input.GetAxis("Mouse Y") * zoomLevelsMouseSensitivity[currentZoomLevel];
                ProcessCameraMovement(inputX, inputY);
                // scroll = change fov zoom level and apply respective depth of field
                if (Input.mouseScrollDelta != Vector2.zero)
                {
                    currentZoomLevel += Math.Sign(Input.mouseScrollDelta.y);
                    currentZoomLevel = Math.Clamp(currentZoomLevel, 0, zoomLevels.Length - 1);

                    cam.fieldOfView = zoomLevels[currentZoomLevel];
                    dofShaderMat.SetFloat("_DepthOfFieldSize", zoomLevelsDepthOfField[currentZoomLevel]);
                }
            }
            else
            {
                if (Input.mouseScrollDelta != Vector2.zero)
                {
                    peripheryBlurRadius = Math.Clamp(peripheryBlurRadius - Input.mouseScrollDelta.y * 0.1f, 0.1f, maxPeripheryBlurRadius);
                    dofShaderMat.SetFloat("_PeripheryBlurRadius", peripheryBlurRadius);
                }
            }

            // W/S = change focus distance
            int focalDepthDirection = 0;
            if (Input.GetKey(KeyCode.W)) focalDepthDirection += 1;
            if (Input.GetKey(KeyCode.S)) focalDepthDirection -= 1;
            float fD = dofShaderMat.GetFloat("_FocusDistance") + focalDepthDirection * focusDistanceSpeed * Time.deltaTime;
            fD = Math.Clamp(fD, minFocusDistance, maxFocusDistance);
            dofShaderMat.SetFloat("_FocusDistance", fD);

            // photo
            if (Input.GetMouseButtonDown(0))
            {
                if (!MissionManager.sharedInstance.isTutorial || (MissionManager.sharedInstance.isTutorial && TutorialManager.sharedInstance.currentIndex > 7))
                {
                    if (focusMode)
                    {
                        DetectBirdsOnPhoto(true);
                        StartCoroutine(TakePhotoScreenshotWithFeedback());
                        GetComponent<AudioSource>().Play();
                        TrackTapeAmount();
                        GameManager.sharedInstance.hasEvidence = true;
                        //focusMode = false;
                        //cam.fieldOfView = zoomLevels[currentZoomLevel]; -> moved to end of photo animation in TakePhotoScreenshotWithFeedback
                    }
                    else
                    {
                        focusMode = true;
                        cam.fieldOfView *= 0.9f;
                    }
                }

            }
            // cancel photo
            if (focusMode && Input.GetMouseButtonDown(1))
            {
                resetAfterFocusMode();
            }
        }

        if (MissionManager.sharedInstance.isTutorial && TutorialManager.sharedInstance.currentIndex == 14)
        {
            DetectBirdsOnPhoto(false);
        }
        if (MissionManager.sharedInstance.isTutorial && TutorialManager.sharedInstance.currentIndex >10 && TutorialManager.sharedInstance.currentIndex < 15)
        {
            CheckRealBirdInFocus();
        }

        TrackTime();
    }

    private bool IsWithinFocusedArea(GameObject obj)
    {
        Material dofShaderMat = effectScript.dofMat;

        float distance = Vector3.Distance(obj.transform.position, transform.position);

        float focusDistance = dofShaderMat.GetFloat("_FocusDistance");
        float dof = dofShaderMat.GetFloat("_DepthOfFieldSize");
        return Math.Abs(distance - focusDistance) < dof/2;
    }

    private bool IsInPeriphery(GameObject obj)
    {
        float angle = Vector3.Angle(cam.transform.forward, obj.transform.position - transform.position);
        bool isInPeriphery = angle > cam.fieldOfView * peripheryBlurRadius;
        return isInPeriphery;
    }

    private bool isObstructed(GameObject obj, out RaycastHit hit)
    {
        bool didHit = Physics.Linecast(transform.position, obj.transform.position, out hit);
        Debug.DrawLine(transform.position, didHit ? hit.point : obj.transform.position, Color.cyan, float.MaxValue);
        return didHit;
    }
    private bool isObstructed(GameObject obj)
    {
        var hit = new RaycastHit();
        return isObstructed(obj, out hit);
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

    void CheckRealBirdInFocus()
    {
        GameObject[] realBirds = GameObject.FindGameObjectsWithTag("RealBird");
        realBirdInFocus = false;
        for (int i = 0; i < realBirds.Length; i++)
        {
            GameObject b = realBirds[i];
            realBirdInFocus |= b.GetComponent<MeshRenderer>().isVisible && IsWithinFocusedArea(b) && !IsInPeriphery(b) && !isObstructed(b);
            if (!realBirdInFocus)
            {
                if (MissionManager.sharedInstance.isTutorial && TutorialManager.sharedInstance.currentIndex < 14)
                {
                    Debug.Log("NOOrealfocus");
                    TutorialManager.sharedInstance.lookingAtNormal = false;
                }
            } else
            {
                if (MissionManager.sharedInstance.isTutorial && TutorialManager.sharedInstance.currentIndex < 14)
                {
                    Debug.Log("realfocus");
                    TutorialManager.sharedInstance.lookingAtNormal = true;
                    
                }
                else
                {
                    break;
                }
                
            }

            
        }
    }

    void DetectBirdsOnPhoto(bool isFullDetection)
    {
        GameObject[] roboBirds = GameObject.FindGameObjectsWithTag("RobotBird");
        foreach (GameObject rb in roboBirds)
        {
            // is a visible pigeon showing suspicious behaviour?
            var robotScript = rb.GetComponent<AIRobotController>();
            if (rb.GetComponent<MeshRenderer>() && robotScript.isSpying)
            {
                // test if pigeon is obstructed
                RaycastHit hit;
                bool didHit = isObstructed(rb, out hit);
                if (!didHit)
                {
                    // test if pigeon is in focus
                    if (IsWithinFocusedArea(rb) && !IsInPeriphery(rb))
                    {
                        // test if other (real) birds are in focus
                        CheckRealBirdInFocus();
                        if (!realBirdInFocus)
                        {
                            if (isFullDetection)
                            {
                                correctPhotosAmount++;
                                GameManager.sharedInstance.hasCorrectPhotos = true;
                                Debug.Log("sus bird on photo " + correctPhotosAmount);

                                if (MissionManager.sharedInstance.isTutorial)
                                {
                                    TutorialManager.sharedInstance.showRobot = false;
                                    Debug.Log("CHANGE SHOW ROBOT");
                                }

                            } else
                            {
                                
                            }
                                
                        }
                        else
                        {
                            Debug.Log("sus bird on photo, but real bird too");
                        }
                    }
                    else
                    {
                        Debug.Log("sus bird out of focus");
                    }

                }
                else
                {
                    Debug.Log("sus bird obstructed ");
                    Debug.Log(hit.collider.name);
                }
            }
            else if (rb.GetComponent<MeshRenderer>().isVisible && !robotScript.isSpying)
            {
                if (isFullDetection)
                {
                    Debug.Log("wrong timing");
                } else
                {
                    
                    if (MissionManager.sharedInstance.isTutorial)
                    {
                        Debug.Log("visible");
                        TutorialManager.sharedInstance.hintRobot = true;
                        Invoke("ShowTutorialRobot", 3f);
                    }
                    
                }
                    
            }
        }
    }

    void ShowTutorialRobot()
    {
        TutorialManager.sharedInstance.showRobot = true;
    }

    void resetAfterFocusMode()
    {
        focusMode = false;
        cam.fieldOfView = zoomLevels[currentZoomLevel];
        peripheryBlurRadius = maxPeripheryBlurRadius;

        Material dofShaderMat = effectScript.dofMat;
        dofShaderMat.SetFloat("_PeripheryBlurRadius", peripheryBlurRadius);
    }

    void TrackTapeAmount()
    {
        TapeManager.instance.AddUsedTape();

        if (TapeManager.instance.reachedLimit)
        {
            //ControlCorrectPhotos(); TODO: only in the last level, add level check
            GameManager.sharedInstance.TriggerGameOver(); //only in levels before the last
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
        if (cam == null) cam = GetComponent<Camera>();
        // absolute values
        float minHRot = (Application.isPlaying ? referenceHorizontalRotation : transform.localEulerAngles.y) + minHorizontalRotation;
        float maxHRot = (Application.isPlaying ? referenceHorizontalRotation : transform.localEulerAngles.y) + maxHorizontalRotation;
        float minVRot = (Application.isPlaying ? referenceVerticalRotation : transform.localEulerAngles.x) + minVerticalRotation;
        float maxVRot = (Application.isPlaying ? referenceVerticalRotation : transform.localEulerAngles.x) + maxVerticalRotation;

        float fovVert = maxVRot - minVRot;
        float fovHori = maxHRot - minHRot;

        // add the camera's fov, so we can see the actual edges of what can be seen
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

    IEnumerator TakePhotoScreenshotWithFeedback()
    {
        photoAnimationInProgress = true;
        yield return null;
        //deactivate UI for photo
        TapeManager.instance.gameObject.GetComponent<Canvas>().enabled = false;
        yield return new WaitForEndOfFrame();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        //reactivate UI for photo
        TapeManager.instance.gameObject.GetComponent<Canvas>().enabled = true;
        // save image
        if (savePhotos)
        {
            byte[] bytes = texture.EncodeToPNG();
            var dirPath = Application.dataPath + "/../SavedPhotos/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            // only save the last 20 pictures
            int photoNumber = PlayerPrefs.GetInt("LastPhotoNumber", 0);
            photoNumber = (photoNumber + 1) % 20;
            // start with Photo1.pgn (not Photo0.png)
            File.WriteAllBytes(dirPath + "Photo" + (photoNumber + 1) + ".png", bytes);
            PlayerPrefs.SetInt("LastPhotoNumber", photoNumber);
            PlayerPrefs.Save();
        }

        GameManager.sharedInstance.AddPhotoToGallery(texture);
        // set the scale to fill the screen
        Vector3 imageScale = new Vector3(texture.width / 100.0f, texture.height / 100.0f, 1.0f);
        RectTransform imageRect = screenshotImage.gameObject.GetComponent<RectTransform>();
        imageRect.localScale = imageScale;
        flashImage.gameObject.GetComponent<RectTransform>().localScale = imageScale;
        // set the image to bottom left corner to prepare for the minimizing animation
        imageRect.anchoredPosition = new Vector3(-texture.width/2, -texture.height/2, 0);
        ApplyGammaCorrection(texture);
        screenshotImage.texture = texture;
        screenshotImage.gameObject.SetActive(true);

        // flash animation (image is white since texture is not set yet)
        flashImage.gameObject.SetActive(true);
        float timeElapsed = 0;
        float flashUpTime = 0.1f;
        float flashDownTime = 0.35f;
        while (timeElapsed < flashUpTime + flashDownTime)
        {
            if (timeElapsed < flashUpTime)
            {
                flashImage.color = new Color(1, 1, 1, Mathf.Lerp(0f, 1f, timeElapsed / flashUpTime));
            }
            else
            {
                flashImage.color = new Color(1, 1, 1, Mathf.Lerp(1f, 0f, (timeElapsed - flashUpTime) / flashDownTime));
            }
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        flashImage.gameObject.SetActive(false);

        // continue showing the photo for a time
        yield return new WaitForSeconds(2);
        //resetAfterFocusMode();
        photoAnimationInProgress = false;
        // minimize image animation
        timeElapsed = 0;
        float minimizeTime = 0.3f;
        while (timeElapsed < minimizeTime)
        {
            imageRect.localScale = imageScale * Mathf.Lerp(1f, 0f, timeElapsed / minimizeTime);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        GameManager.sharedInstance.UpdatePhotoPreview(texture);

        // cleanup
        screenshotImage.gameObject.SetActive(false);
        screenshotImage.texture = null;
        //UnityEngine.Object.Destroy(texture);
    }

    private static void ApplyGammaCorrection(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(Mathf.GammaToLinearSpace(pixels[i].r),
                                   Mathf.GammaToLinearSpace(pixels[i].g),
                                   Mathf.GammaToLinearSpace(pixels[i].b),
                                   pixels[i].a);
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }
}
