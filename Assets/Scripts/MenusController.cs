using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    private int okCheck = 0;
    private bool isReturnPressed = false;

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

        if (upButton != null)
        {
            upButton.onClick.RemoveAllListeners();
            upButton.onClick.AddListener(() => OnUpButtonClicked());
        }

        if (downButton != null)
        {
            downButton.onClick.RemoveAllListeners(); 
            downButton.onClick.AddListener(() => OnDownButtonClicked());
        }

        if (leftButton != null)
        {
            leftButton.onClick.RemoveAllListeners();
            leftButton.onClick.AddListener(() => OnLeftButtonClicked());
        }

        if (rightButton != null)
        {
            rightButton.onClick.RemoveAllListeners();
            rightButton.onClick.AddListener(() => OnRightButtonClicked());
        }

        if (okButton != null)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(() => PressOKButton());
        }
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
            AssignHorizKeyboard();
            AssignVerticKeyboard();

        } else
        {
            upButton.interactable = !isHorizontal;
            downButton.interactable = !isHorizontal;
            leftButton.interactable = isHorizontal;
            rightButton.interactable = isHorizontal;

            if (isHorizontal) AssignHorizKeyboard(); else AssignVerticKeyboard();
            
        }

        if (Input.GetKeyDown(KeyCode.Return) && !isReturnPressed)
        {
            Debug.Log("Return key pressed");
            isReturnPressed = true;
            PressButton(okButton);
            PressOKButton();
        }
        else if (isReturnPressed && !Input.GetKey(KeyCode.Return))
        {
            Debug.Log("Return key released (manual detection)");
            isReturnPressed = false;
            ReleaseButton(okButton);
        }
    }

    private void AssignHorizKeyboard()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "NextLevelScene" || GameManager.sharedInstance.isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnLeftButtonClicked();
                PressButton(leftButton);
            }
            else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                ReleaseButton(leftButton);
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnRightButtonClicked();
                PressButton(rightButton);
            }
            else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                ReleaseButton(rightButton);
            }
        }
    }

    private void AssignVerticKeyboard()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "NextLevelScene" || GameManager.sharedInstance.isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("Down key pressed");
                OnDownButtonClicked();
                PressButton(downButton);
            }
            else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                Debug.Log("Down key released");
                ReleaseButton(downButton);
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log("Up key pressed");
                OnUpButtonClicked();
                PressButton(upButton);
            }
            else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                Debug.Log("Up key released");
                ReleaseButton(upButton);
            }
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
        Debug.Log("menucheck previous");
        List<MenuOption> options;
        options = isHorizontal ? horizOptions : verticOptions;

        do
        {
            selection = (selection - 1 + options.Count) % options.Count;
        } while (!options[selection].sprite.activeInHierarchy &&
           !options[selection].selectedSprite.activeInHierarchy);

        UpdateSelection(options);

    }

    public void SelectNext()
    {
        Debug.Log("menucheck next");
        List<MenuOption> options;
        options = isHorizontal ? horizOptions : verticOptions;
        do
        {
            selection = (selection + 1) % options.Count;
        } while (!options[selection].sprite.activeInHierarchy &&
             !options[selection].selectedSprite.activeInHierarchy);

        UpdateSelection(options);

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
            okCheck++;
            Debug.Log("okokokokokkokok " + okCheck);
        }  
    }

    public void OnUpButtonClicked()
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

    public void OnDownButtonClicked()
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
            options[i].sprite.SetActive(i != selection);
            options[i].selectedSprite.SetActive(i == selection);
          
        }
    }

    private void PressButton(Button button)
    {
        var clickDown = new PointerEventData(eventSystem);
        ExecuteEvents.Execute(button.gameObject, clickDown, ExecuteEvents.pointerDownHandler);
    }
    private void ReleaseButton(Button button)
    {
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
