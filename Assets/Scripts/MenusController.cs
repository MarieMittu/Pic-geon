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
        PhotoGrid, // Mode for photo grid navigation
        OptionsMenu // Mode for vertical/horizontal options menu
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
    public Transform photoGridContent; // The Content GameObject of the grid
    private List<SelectablePhoto> photos = new List<SelectablePhoto>();
    public int photoColumns = 3; // Set the number of columns for the photo grid
    private int currentGridIndex = 0; // Tracks the currently selected photo


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

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnUpButtonClicked();
                ClickCameraButton(upButton);
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnDownButtonClicked();
                ClickCameraButton(downButton);
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnLeftButtonClicked();
                ClickCameraButton(leftButton);
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnRightButtonClicked();
                ClickCameraButton(rightButton);
            }

        } else
        {
            upButton.interactable = !isHorizontal;
            downButton.interactable = !isHorizontal;
            leftButton.interactable = isHorizontal;
            rightButton.interactable = isHorizontal;

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {

                ClickCameraButton(downButton);
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnUpButtonClicked();
                ClickCameraButton(upButton);
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ClickCameraButton(leftButton);
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                ClickCameraButton(rightButton);
            }
        }
           
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ClickCameraButton(okButton);
        }
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
        List<MenuOption> options;
        options = isHorizontal ? horizOptions : verticOptions;
        if (selection >= 0 && selection < options.Count)
        {
            options[selection].onSelect.Invoke();
        }
    }

    private void OnUpButtonClicked()
    {
        if (currentMode == MenuMode.PhotoGrid)
        {
            MoveGridSelection(-1, 0); // Navigate up in the photo grid
        }
        else
        {
            SelectPrevious(); // Navigate up in the vertical options
        }
    }

    // Handle UI button click for "Down" button
    private void OnDownButtonClicked()
    {
        if (currentMode == MenuMode.PhotoGrid)
        {
            MoveGridSelection(1, 0); // Navigate down in the photo grid
        }
        else
        {
            SelectNext(); // Navigate down in the vertical options
        }
    }

    // Handle UI button click for "Left" button
    private void OnLeftButtonClicked()
    {
        if (currentMode == MenuMode.PhotoGrid)
        {
            MoveGridSelection(0, -1); // Navigate left in the photo grid
        }
        else
        {
            SelectPrevious(); // Navigate left in the horizontal options
        }
    }

    // Handle UI button click for "Right" button
    private void OnRightButtonClicked()
    {
        if (currentMode == MenuMode.PhotoGrid)
        {
            MoveGridSelection(0, 1); // Navigate right in the photo grid
        }
        else
        {
            SelectNext(); // Navigate right in the horizontal options
        }
    }

    private void UpdateSelection(List<MenuOption> options)
    {
        
            for (int i = 0; i < options.Count; i++)
            {
                options[i].sprite.SetActive(i != selection);
                options[i].selectedSprite.SetActive(i == selection);
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
                photo.DeselectPhoto(); // Deselect all photos initially
            }
        }

        // Select the first photo by default
        if (photos.Count > 0)
        {
            currentMode = MenuMode.PhotoGrid;
            currentGridIndex = 0;
            photos[currentGridIndex].SelectPhoto();
        }
    }

    // Move photo selection in the grid
    private void MoveGridSelection(int rowDelta, int columnDelta)
    {
        if (photos.Count == 0) return; // No photos to navigate

        // Calculate new grid index
        int currentRow = currentGridIndex / photoColumns;
        int currentColumn = currentGridIndex % photoColumns;

        int newRow = Mathf.Clamp(currentRow + rowDelta, 0, Mathf.CeilToInt((float)photos.Count / photoColumns) - 1);
        int newColumn = Mathf.Clamp(currentColumn + columnDelta, 0, photoColumns - 1);

        int newIndex = newRow * photoColumns + newColumn;

        // Ensure the new index is within the bounds of the photo list
        if (newIndex >= photos.Count) return;

        // Update the selection
        photos[currentGridIndex].DeselectPhoto(); // Deselect the current photo
        currentGridIndex = newIndex;
        photos[currentGridIndex].SelectPhoto();   // Select the new photo
    }

}
