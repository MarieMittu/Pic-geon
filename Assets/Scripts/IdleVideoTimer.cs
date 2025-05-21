using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IdleVideoTimer : MonoBehaviour
{
    float timeElapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (Input.anyKeyDown) timeElapsed = 0;
        if (timeElapsed > (2 * 60)) {
            SceneManager.LoadScene("IdleVideo");
            Debug.Log("Transitioning to IdleVideo scene");
        }
    }
}
