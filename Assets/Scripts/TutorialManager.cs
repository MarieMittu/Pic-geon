using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public static TutorialManager sharedInstance;

    public GameObject[] tutorials;
    public float focusTimer = 6;
    public GameObject normalMarker;
    public GameObject robotMarker;

    [HideInInspector]
    public int currentIndex = 0;
    public bool showRobot = false;
    public bool hintRobot = false;
    public bool lookingAtNormal = false;

    private bool isSwitching = false;
    private Dictionary<int, int> tutorialSwitchMap;

    private void Awake()
    {

        sharedInstance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        MissionManager.sharedInstance.isTutorial = true;
        tutorialSwitchMap = new Dictionary<int, int>
        {
            { 0, 1 },
            { 3, 4 },
            { 5, 6 },
            { 7, 8 },
            { 9, 10 },
            { 13, 14 },
            { 18, 19 },
            { 19, 20 },
            { 20, 21 },
            { 21, 22 },
            { 22, 23 }
        };

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
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShowNextTutorial(7);
        }
        if (Input.GetMouseButtonDown(0))
        {
            ShowNextTutorial(9);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentIndex == 11) normalMarker.SetActive(true);
            ShowNextTutorial(12);
        }

        if (currentIndex == 12)
        {
            if (lookingAtNormal)
            {
                focusTimer -= Time.deltaTime;

                if (focusTimer <= 0)
                {
                    ShowNextTutorial(13);
                    
                    lookingAtNormal = false;
                }
            }
            
        }

        if (currentIndex == 13)
        {
            normalMarker.SetActive(false);
            robotMarker.SetActive(true);
        }

        if (hintRobot)
        {
            if (currentIndex == 14)
            {
                ShowNextTutorial(15);
            }
        }

        if (showRobot)
        {
           
            if (currentIndex == 15)
            {
                ShowNextTutorial(16);
            }
            if (currentIndex == 16)
            {
                ShowNextTutorial(17);
                
            }
        } else
        {
            if (currentIndex == 17)
            {
                robotMarker.SetActive(false);
                ShowNextTutorial(18);

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
        if (tutorialSwitchMap.TryGetValue(currentIndex, out int nextIndex))
        {
            ActivateTutorial(nextIndex);
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
