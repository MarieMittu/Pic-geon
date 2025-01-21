using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameObject[] tutorials;
    private int currentIndex = 0;
    private bool isSwitching = false;

    // Start is called before the first frame update
    void Start()
    {
        ActivateTutorial(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchTutorial();

        }
        if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))
        {
            ShowNextTutorial(2);
        }
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            ShowNextTutorial(3);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            ShowNextTutorial(5);
        }
    }

    private void ActivateTutorial(int index)
    {
        if (index < 0 || index >= tutorials.Length) return;

        foreach (var tutorial in tutorials)
        {
            tutorial.SetActive(false);
        }

        tutorials[index].SetActive(true);
        currentIndex = index;
        isSwitching = false;
    }

    private void SwitchTutorial()
    {
        if (currentIndex == 0)
        {
            ActivateTutorial(1);
        }
        else if (currentIndex == 3)
        {
            ActivateTutorial(4);
        }
    }

    private void ShowNextTutorial(int index)
    {
        if (currentIndex == index - 1 && !isSwitching)
        {
            StartCoroutine(PrepareSwitch(index, 2f));
        }
    }

    private IEnumerator PrepareSwitch(int index, float delay)
    {
        isSwitching = true; 
        yield return new WaitForSeconds(delay); 
        ActivateTutorial(index);
    }
}
