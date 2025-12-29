using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class TransSceneTrigger : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;
    public VideoClip trueEndVideo;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger Next Scene");
            videoPlayer.clip = trueEndVideo;
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("BackgroundChasing");
    }
}
