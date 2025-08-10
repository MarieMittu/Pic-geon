using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public static TutorialManager sharedInstance;

    [Header("Tutorial Settings")]
    public int baseStepOffset = 0; // Set to 0 for part1, 19 for part2
    


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

    public int GlobalIndex => baseStepOffset + currentIndex;

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
            { 13, 14 },
            //part 2
            { 19, 20 },
            { 20, 21 },
            { 21, 22 },
            { 23, 24 },
            { 24, 25 },
            { 27, 28 },
            { 28, 29 },
            { 31, 32 },
            { 32, 33 },
            { 33, 34 },
            { 34, 35 },
            { 35, 36 },
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
            ShowNextTutorialGlobal(3);
        }
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            if (GlobalIndex == 3)
            {
                ShowNextTutorialGlobal(4);
            } else if (GlobalIndex == 9)
            {
                ShowNextTutorialGlobal(10);
            }
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            ShowNextTutorialGlobal(6);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ShowNextTutorialGlobal(23);
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (GlobalIndex == 7) ShowNextTutorialGlobal(8);
            if (GlobalIndex == 10)
            {
                ShowNextTutorialGlobal(11);
                foreach (GameObject marker in normalMarkers)
                {
                    marker.SetActive(true);
                }
                PositionNormalMarkers();
            }
               
        }
  
            

        if (GlobalIndex == 11)
        {
            if (lookingAtNormal)
            {
                focusTimer -= Time.deltaTime;

                if (focusTimer <= 0)
                {
                    ShowNextTutorialGlobal(12);
                    
                    lookingAtNormal = false;
                }
            }
            
        }

        if (GlobalIndex == 14)
        {
            foreach (GameObject marker in normalMarkers)
            {
                marker.SetActive(false);
            }
            ShowNextTutorialGlobal(15);

        }

        if (hintRobot)
        {
            if (GlobalIndex == 15)
            {

                robotMarker.SetActive(true);
                PositionRobotMarker();
            }
        }

        if (showRobot)
        {
           
            if (GlobalIndex == 15)
            {
                ShowNextTutorialGlobal(16);
            }
            if (GlobalIndex == 16)
            {
                ShowNextTutorialGlobal(17);
                
            }
        } else
        {
            if (GlobalIndex == 17)
            {
                robotMarker.SetActive(false);
                ShowNextTutorialGlobal(18);

            }
        }
        if (GlobalIndex == 18)
        {
            Invoke("GoToWinScene", 3f);
        }

        if (GlobalIndex == 25)
        {
            foreach (GameObject marker in normalMarkers)
            {
                marker.SetActive(true);
            }
            PositionNormalMarkers();
            ShowNextTutorialGlobal(26);
        }

        if (GlobalIndex == 26 && Input.GetMouseButtonDown(0))
        {
            foreach (GameObject marker in normalMarkers)
            {
                marker.SetActive(false);
            }
            ShowNextTutorialGlobal(27);
        }

        if (GlobalIndex == 29)
        {
            robotMarker.SetActive(true);
            PositionRobotMarker();
        }
        if (GlobalIndex == 31)
        {
            robotMarker.SetActive(false);
        }
    }



    public void ShowNextTutorialGlobal(int globalIndex)
    {
        int localIndex = globalIndex - baseStepOffset;
        ShowNextTutorial(localIndex);
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
        if (tutorialSwitchMap.TryGetValue(GlobalIndex, out int nextGlobalIndex))
        {
            int localIndex = nextGlobalIndex - baseStepOffset;
            ActivateTutorial(localIndex);
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
