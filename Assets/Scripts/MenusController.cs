using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenusController : MonoBehaviour
{

    [System.Serializable]
    public class MenuOption
    {
        public GameObject sprite;         
        public GameObject selectedSprite; 
        public UnityEngine.Events.UnityEvent onSelect; 
    }

    public enum MenuMode
    {
        PhotoGrid, 
        OptionsMenu 
    }

    public MenuMode currentMode = MenuMode.OptionsMenu;

    public List<MenuOption> verticOptions = new List<MenuOption>();
    public List<MenuOption> horizOptions = new List<MenuOption>();
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    public Button okButton;

    private EventSystem eventSystem;
    private int selection = 0;

    public bool isHorizontal = false;

    // for photo library
    public Transform photoGridContent; 
    private List<SelectablePhoto> photos = new List<SelectablePhoto>();
    public int photoColumns = 3; 
    private int currentGridIndex = 0; 


    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        SetupOptions();

        if (upButton != null) upButton.onClick.AddListener(() => OnUpButtonClicked());
        if (downButton != null) downButton.onClick.AddListener(() => OnDownButtonClicked());
        if (leftButton != null) leftButton.onClick.AddListener(() => OnLeftButtonClicked());
        if (rightButton != null) rightButton.onClick.AddListener(() => OnRightButtonClicked());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMode == MenuMode.PhotoGrid && photoGridContent != null && photoGridContent.gameObject.activeInHierarchy)
        {
            upButton.interactable = true;
            downButton.interactable = true;
            leftButton.interactable = true;
            rightButton.interactable = true;

            //if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            //{
            //    OnUpButtonClicked();
            //    ClickCameraButton(upButton);
            //}
            //if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    OnDownButtonClicked();
            //    ClickCameraButton(downButton);
            //}
            //if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            //{
            //    OnLeftButtonClicked();
            //    ClickCameraButton(leftButton);
            //}
            //if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            //{
            //    OnRightButtonClicked();
            //    ClickCameraButton(rightButton);
            //}

        } else
        {
            upButton.interactable = !isHorizontal;
            downButton.interactable = !isHorizontal;
            leftButton.interactable = isHorizontal;
            rightButton.interactable = isHorizontal;

            //if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    OnDownButtonClicked();
            //    ClickCameraButton(downButton);
            //}

            //if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            //{
            //    OnUpButtonClicked();
            //    ClickCameraButton(upButton);
            //}

            //if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            //{
            //    OnLeftButtonClicked();
            //    ClickCameraButton(leftButton);
            //}

            //if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            //{
            //    OnRightButtonClicked();
            //    ClickCameraButton(rightButton);
            //}
        }
           
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    ClickCameraButton(okButton);
        //}
    }

    public void SetupOptions()
    {
        List<MenuOption> options;
        options = isHorizontal ? horizOptions : verticOptions;
        for (int i = 0; i < options.Count; i++)
        {
            options[i].sprite.SetActive(i != selection);
            options[i].selectedSprite.SetActive(i == selection);
        }
    }

    public void SelectPrevious()
    {
        List<MenuOption> options;
        options = isHorizontal ? horizOptions : verticOptions;

            if (options[1].sprite.activeInHierarchy || options[1].selectedSprite.activeInHierarchy)
            {
                selection = (selection - 1 + options.Count) % options.Count;
                UpdateSelection(options);
            }
         
    }

    public void SelectNext()
    {
        List<MenuOption> options;
        options = isHorizontal ? horizOptions : verticOptions;
        if (options[1].sprite.activeInHierarchy || options[1].selectedSprite.activeInHierarchy)
        {
            selection = (selection + 1) % options.Count;
            UpdateSelection(options);
        }

    }

    public void PressOKButton()
    {
        if (currentMode == MenuMode.PhotoGrid)
        {
            ActivateBigPhoto();
        } else
        {
            List<MenuOption> options;
            options = isHorizontal ? horizOptions : verticOptions;
            if (selection >= 0 && selection < options.Count)
            {
                options[selection].onSelect.Invoke();
            }
        }  
    }

    private void OnUpButtonClicked()
    {
        if (currentMode == MenuMode.PhotoGrid)
        {
            MoveGridSelection(-1, 0); 
        }
        else
        {
            SelectPrevious(); 
        }
    }

    private void OnDownButtonClicked()
    {
        if (currentMode == MenuMode.PhotoGrid)
        {
            MoveGridSelection(1, 0);
        }
        else
        {
            SelectNext(); 
        }
    }

    private void OnLeftButtonClicked()
    {
        if (currentMode == MenuMode.PhotoGrid)
        {
            MoveGridSelection(0, -1); 
        }
        else
        {
            SelectPrevious(); 
        }
    }

    private void OnRightButtonClicked()
    {
        if (currentMode == MenuMode.PhotoGrid)
        {
            MoveGridSelection(0, 1); 
        }
        else
        {
            SelectNext(); 
        }
    }

    private void UpdateSelection(List<MenuOption> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].sprite == null || options[i].selectedSprite == null)
            {
                Debug.Log($"Invalid MenuOption at index {i}: Missing sprite or selectedSprite.");
                continue;
            }

            bool isSelected = (i == selection);

            // Force-reset states to prevent stale visibility
            options[i].sprite.SetActive(false);
            options[i].selectedSprite.SetActive(false);

            // Apply correct visibility
            options[i].sprite.SetActive(!isSelected);
            options[i].selectedSprite.SetActive(isSelected);

            Debug.Log($"Option {i}: sprite {(isSelected ? "hidden" : "visible")}, selectedSprite {(isSelected ? "visible" : "hidden")}");
            Debug.Log($"UpdateSelection called: selection = {selection}");
        }
    }


    private void ClickCameraButton(Button button)
    {
        StartCoroutine(ClickAnimation(button));
        button.onClick.Invoke();
    }

    private IEnumerator ClickAnimation(Button button)
    {
        var clickDown = new PointerEventData(eventSystem);
        ExecuteEvents.Execute(button.gameObject, clickDown, ExecuteEvents.pointerDownHandler);

        yield return new WaitForSeconds(0.1f);

        var clickUp = new PointerEventData(eventSystem);
        ExecuteEvents.Execute(button.gameObject, clickUp, ExecuteEvents.pointerUpHandler);
    }

    // for photo library

    public void InitializePhotoGrid()
    {
        photos.Clear();

        foreach (Transform child in photoGridContent)
        {
            SelectablePhoto photo = child.GetComponent<SelectablePhoto>();
            if (photo != null)
            {
                photos.Add(photo);
                photo.DeselectPhoto(); 
            }
        }

        if (photos.Count > 0)
        {
            currentMode = MenuMode.PhotoGrid;
            currentGridIndex = 0;
            photos[currentGridIndex].SelectPhoto();
        }
    }

    private void MoveGridSelection(int rowDelta, int columnDelta)
    {
        if (photos.Count == 0) return; 

        int currentRow = currentGridIndex / photoColumns;
        int currentColumn = currentGridIndex % photoColumns;

        int newRow = Mathf.Clamp(currentRow + rowDelta, 0, Mathf.CeilToInt((float)photos.Count / photoColumns) - 1);
        int newColumn = Mathf.Clamp(currentColumn + columnDelta, 0, photoColumns - 1);

        int newIndex = newRow * photoColumns + newColumn;

        if (newIndex >= photos.Count) return;

        photos[currentGridIndex].DeselectPhoto(); 
        currentGridIndex = newIndex;
        photos[currentGridIndex].SelectPhoto();  
    }

    private void ActivateBigPhoto()
    {
        if (photos.Count > 0)
        {
            Texture2D selectedTexture = photos[currentGridIndex].GetPhotoTexture();

            GameManager.sharedInstance.bigPhoto.SetActive(true);

            if (GameManager.sharedInstance.bigPhotoImage != null)
            {
                Sprite sprite = Sprite.Create(selectedTexture, new Rect(0, 0, selectedTexture.width, selectedTexture.height), new Vector2(0.5f, 0.5f));
                GameManager.sharedInstance.bigPhotoImage.sprite = sprite;
            }
        }
    }
}
