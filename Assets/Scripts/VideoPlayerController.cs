using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var video = GetComponent<VideoPlayer>();
        if (Input.GetKeyDown(KeyCode.M)) video.SetDirectAudioMute(0, !video.GetDirectAudioMute(0));
        else if (Input.anyKeyDown) SceneManager.LoadScene("IntroScene");
    }
}
