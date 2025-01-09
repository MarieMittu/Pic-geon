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

    public List<MenuOption> options = new List<MenuOption>();
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    public Button okButton;

    private EventSystem eventSystem;
    private int selection = 0;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        Debug.Log("optionsmenu " + options[1].sprite.activeInHierarchy);
        for (int i = 0; i < options.Count; i++)
        {
            options[i].sprite.SetActive(i != selection);
            options[i].selectedSprite.SetActive(i == selection);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ClickCameraButton(downButton);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ClickCameraButton(upButton);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ClickCameraButton(okButton);
        }
    }

    public void PressUpButton()
    {
        if (options[1].sprite.activeInHierarchy || options[1].selectedSprite.activeInHierarchy)
        {
            selection = (selection - 1 + options.Count) % options.Count;
            UpdateSelection();
        }
        
    }

    public void PressDownButton()
    {
        if (options[1].sprite.activeInHierarchy || options[1].selectedSprite.activeInHierarchy)
        {
            selection = (selection + 1) % options.Count;
            UpdateSelection();
        }
            
    }

    public void PressOKButton()
    {
        if (selection >= 0 && selection < options.Count)
        {
            options[selection].onSelect.Invoke();
        }
    }

    private void UpdateSelection()
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
