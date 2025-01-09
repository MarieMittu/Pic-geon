using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    float selection;

    [Space(10)]
    [Header("Mission 1")]
    public GameObject mission1Sprite;
    public GameObject mission1Selected;

    [Space(10)]
    [Header("Mission 2")]
    public GameObject mission2Sprite;
    public GameObject mission2Selected;

    [Space(10)]
    [Header("Mission 3")]
    public GameObject mission3Sprite;
    public GameObject mission3Selected;

    [Space(10)]
    [Header("Buttons")]
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    public Button okButton;

    private EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        selection = 1;
        eventSystem = EventSystem.current;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ClickCameraButton(downButton);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ClickCameraButton(upButton);
        }

        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    ClickCameraButton(okButton);
        //}

        if (selection == 1)
        {
            mission1Sprite.SetActive(false);
            mission1Selected.SetActive(true);
            mission2Sprite.SetActive(true);
            mission2Selected.SetActive(false);
            mission3Sprite.SetActive(true);
            mission3Selected.SetActive(false);
        }

        if (selection == 2)
        {
            mission1Sprite.SetActive(true);
            mission1Selected.SetActive(false);
            mission2Sprite.SetActive(false);
            mission2Selected.SetActive(true);
            mission3Sprite.SetActive(true);
            mission3Selected.SetActive(false);
        }

        if (selection == 3)
        {
            mission1Sprite.SetActive(true);
            mission1Selected.SetActive(false);
            mission2Sprite.SetActive(true);
            mission2Selected.SetActive(false);
            mission3Sprite.SetActive(false);
            mission3Selected.SetActive(true);
        }
    }

    public void PressUpButton()
    {
        if (selection >= 1) selection--;
        if (selection < 1) selection = 3;
    }

    public void PressDownButton()
    {
        if (selection <= 3) selection++;
        if (selection > 3) selection = 1;
    }

    public void PressOKButton()
    {
        
        if (selection == 1)
        {
            StartCoroutine(PlayGame());
        }

        if (selection == 2)
        {
            StartCoroutine(PlayGame());
        }

        if (selection == 3)
        {
            StartCoroutine(PlayGame());
        }
    }

    private IEnumerator PlayGame()
    {
        yield return new WaitForSeconds(0.2f);
        FindObjectOfType<ScenesController>().StartGame();
    }

    private void ClickCameraButton(Button button)
    {
        StartCoroutine(ClickAnimation(button));
        button.onClick.Invoke();
    }

    private System.Collections.IEnumerator ClickAnimation(Button button)
    {
        var clickDown = new PointerEventData(eventSystem);
        ExecuteEvents.Execute(button.gameObject, clickDown, ExecuteEvents.pointerDownHandler);

        yield return new WaitForSeconds(0.1f);

        var clickUp = new PointerEventData(eventSystem);
        ExecuteEvents.Execute(button.gameObject, clickUp, ExecuteEvents.pointerUpHandler);
    }
}
