using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        selection = 1;
    }

    private void Update()
    {
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
            FindObjectOfType<ScenesController>().StartGame();
        }

        if (selection == 2)
        {
            FindObjectOfType<ScenesController>().StartGame();
        }

        if (selection == 3)
        {
            FindObjectOfType<ScenesController>().StartGame();
        }
    }
}
