using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var audio = GetComponent<AudioSource>();
        if (Input.GetKeyDown(KeyCode.M)) audio.enabled = !audio.enabled;
    }
}
