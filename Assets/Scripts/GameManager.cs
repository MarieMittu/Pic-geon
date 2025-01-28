using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;

    public float missionDuration;
    public float startMissionDuration;
    float secondTimer = 0f;
    public Image timer;

    public GameObject noFiles;
    public GameObject preview;

    public GameObject photoSlotPrefab;
    public Transform photoGalleryContent;

    public GameObject bigPhoto;
    public Image bigPhotoImage;

    public bool isGamePaused;
    public bool hasEvidence;
    public bool hasEnoughCorrectPhotos;
    public bool wantsToExit;

    public Camera camera1;
    public Camera camera2;
    public Camera camera3;

    private void Awake()
    {

            sharedInstance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        startMissionDuration = missionDuration;
        if (timer != null)
        {
            timer.fillAmount = 1f;
        }
        hasEvidence = false;
        hasEnoughCorrectPhotos = false;
        //SetUpCameras();

        noFiles.SetActive(true);
        preview.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isGamePaused = FindObjectOfType<ScenesController>().isPaused;

        secondTimer += Time.deltaTime;
        if (secondTimer >= 1f)
        {
            missionDuration--;
            secondTimer -= 1f;

            if (timer != null)
            {
                timer.fillAmount = missionDuration / startMissionDuration;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FindObjectOfType<ScenesController>().isPaused)
            {
                FindObjectOfType<ScenesController>().Resume();
                Cursor.lockState = CursorLockMode.Locked;
            } else
            {
                FindObjectOfType<ScenesController>().Pause();
                if (hasEvidence)
                {
                    FindObjectOfType<ScenesController>().ActivateSubOption();
                    noFiles.SetActive(false);
                    preview.SetActive(true);
                }
            }
        }

        

        FindObjectOfType<MenusController>().isHorizontal = FindObjectOfType<ScenesController>().isAlerting;
        if (FindObjectOfType<MenusController>().isHorizontal)
        {
            FindObjectOfType<MenusController>().SetupOptions();
        }
    }

    private void SetUpCameras()
    {
        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(false);
        camera3.gameObject.SetActive(false);

        switch (MissionManager.sharedInstance.currentMission)
        {
            case 1:
                camera1.gameObject.SetActive(true);
                break;
            case 2:
                camera2.gameObject.SetActive(true);
                break;
            case 3:
                camera3.gameObject.SetActive(true);
                break;
        }
    }


    public void TriggerGameOver()
    {
        FindObjectOfType<ScenesController>().GameOver();
    }

    public void TriggerVictory()
    {
        FindObjectOfType<ScenesController>().GameWon();
    }


    public void TriggerNextLevel()
    {
        FindObjectOfType<ScenesController>().OpenIntermediateScene();
    }

    public void TriggerExitToMenu()
    {
        FindObjectOfType<ScenesController>().GoToMenu();
    }

    public void AlertYesAction()
    {
        if (wantsToExit)
        {
            TriggerExitToMenu();

        } else
        {
            ControlEvidence();
        }
    }

   public void ControlEvidence()
   {
        if (hasEnoughCorrectPhotos)
        {
            if (MissionManager.sharedInstance.currentMission < 3)
            {
                MissionManager.sharedInstance.NextMission();
                TriggerNextLevel();
            } else
            {
                TriggerVictory();
            }
            
        } else
        {
            TriggerGameOver();
        }
   }


    public void UpdatePhotoPreview(Texture2D texture)
    {
        Image previewImage = preview.GetComponent<Image>();

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(texture, rect, pivot);

        previewImage.sprite = sprite;

        preview.SetActive(true);
    }

    public void AddPhotoToGallery(Texture2D photo)
    {
        GameObject newPhotoSlot = Instantiate(photoSlotPrefab, photoGalleryContent);
        SelectablePhoto selectablePhoto = newPhotoSlot.GetComponent<SelectablePhoto>(); 

        if (selectablePhoto != null)
        {
            selectablePhoto.photoTexture = photo; 
        }

        Image photoImage = newPhotoSlot.GetComponent<Image>();
        if (photoImage != null)
        {
            Sprite photoSprite = Sprite.Create(
                photo,
                new Rect(0, 0, photo.width, photo.height),
                new Vector2(0.5f, 0.5f)
            );
            photoImage.sprite = photoSprite;
        }

    }

    public void OpenLibrary()
    {
        FindObjectOfType<ScenesController>().ShowPhotoLibrary();
        FindObjectOfType<MenusController>().InitializePhotoGrid();
    }

}
