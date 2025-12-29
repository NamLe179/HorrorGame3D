using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName;
    [SerializeField] private KeyCode skipKeyCode = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.loopPointReached += EndReached;
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(skipKeyCode))
        {
            LoadNextScene();
        }
    }

    private void EndReached(VideoPlayer source)
    {
        Debug.Log("EndReached");
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
        videoPlayer.loopPointReached -= EndReached;
        Debug.Log("LoadNextScene");
    }
}
