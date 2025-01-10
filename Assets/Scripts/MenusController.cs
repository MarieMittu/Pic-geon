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

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        SetupOptions();
    }

    // Update is called once per frame
    void Update()
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
}
