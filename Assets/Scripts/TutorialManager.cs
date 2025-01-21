using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameObject[] tutorials;
    private int currentIndex = 0;
    private bool isSwitching = false;
    public float focusTimer = 6;

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
            if (currentIndex == 2)
            {
                ShowNextTutorial(3);
            } else if (currentIndex == 10)
            {
                ShowNextTutorial(11);
            }
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            ShowNextTutorial(5);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowNextTutorial(7);
        }
        if (Input.GetMouseButtonDown(0))
        {
            ShowNextTutorial(9);
        }

        if (Input.GetMouseButtonDown(1))
        {
            ShowNextTutorial(12);
        }

        if (currentIndex == 12)
        {
            focusTimer -= Time.deltaTime;

            if (focusTimer <= 0)
            {
                ShowNextTutorial(13);
            }
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
        else if (currentIndex == 5)
        {
            ActivateTutorial(6);
        } else if (currentIndex == 7)
        {
            ActivateTutorial(8);
        } else if (currentIndex == 9)
        {
            ActivateTutorial(10);
        } else if (currentIndex == 13)
        {
            ActivateTutorial(14);
        } else if (currentIndex == 14)
        {
            ActivateTutorial(15);
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
