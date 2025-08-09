using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public static TutorialManager sharedInstance;

    public GameObject[] tutorials;
    public float focusTimer = 6; // time needed to remain focused on normal bird to proceed
    public GameObject[] normalMarkers;
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
            { 1, 2 },
            { 4, 5 },
            { 6, 7 },
            { 8, 9 },
            { 12, 13 },
            { 13, 14 }
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
            ShowNextTutorial(3);
        }
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            if (currentIndex == 3)
            {
                ShowNextTutorial(4);
            } else if (currentIndex == 9)
            {
                ShowNextTutorial(10);
            }
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            ShowNextTutorial(6);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            //ShowNextTutorial(7);
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (currentIndex == 7) ShowNextTutorial(8);
            if (currentIndex == 10)
            {
                ShowNextTutorial(11);
                foreach (GameObject marker in normalMarkers)
                {
                    marker.SetActive(true);
                }
                PositionNormalMarkers();
            }
               
        }
  
            

        if (currentIndex == 11)
        {
            if (lookingAtNormal)
            {
                focusTimer -= Time.deltaTime;

                if (focusTimer <= 0)
                {
                    ShowNextTutorial(12);
                    
                    lookingAtNormal = false;
                }
            }
            
        }

        if (currentIndex == 14)
        {
            foreach (GameObject marker in normalMarkers)
            {
                marker.SetActive(false);
            }
            ShowNextTutorial(15);

        }

        if (hintRobot)
        {
            if (currentIndex == 15)
            {

                robotMarker.SetActive(true);
                PositionRobotMarker();
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
        if (currentIndex == 18)
        {
            Invoke("GoToWinScene", 3f);
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

    private void PositionNormalMarkers()
    {
        GameObject[] realBirds = GameObject.FindGameObjectsWithTag("RealBird");
        int count = Mathf.Min(normalMarkers.Length, realBirds.Length);

        for (int i = 0; i < count; i++)
        {
            Vector3 birdPos = realBirds[i].transform.position;
            Vector3 markerPos = normalMarkers[i].transform.position;
            normalMarkers[i].transform.position = new Vector3(birdPos.x, markerPos.y, birdPos.z);
        }
    }
    private void PositionRobotMarker()
    {
        GameObject robotBird = GameObject.FindGameObjectWithTag("RobotBird");
        if (robotBird != null)
        {
            Vector3 birdPos = robotBird.transform.position;
            Vector3 markerPos = robotMarker.transform.position;
            robotMarker.transform.position = new Vector3(birdPos.x, markerPos.y, birdPos.z);
        }
    }

    private void GoToWinScene()
    {
        GameManager.sharedInstance.TriggerNextLevel();
    }
}
