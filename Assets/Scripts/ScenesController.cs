using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("SampleScene"); // TODO: add camera selection
    }

    public void OpenIntermediateScene()
    {
        SceneManager.LoadScene("NextLevelScene");
    }
}
