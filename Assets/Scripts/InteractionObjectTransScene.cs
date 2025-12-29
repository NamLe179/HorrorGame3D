using HorrorGame3D.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System;

public class InteractionObjectTransScene : InteractSceneLoaderObject
{
    [Header("Monologue Manager")]
    [SerializeField] private MonologueManager monologueManager;
    [SerializeField] private GameObject image;
    [SerializeField] private GameObject managerPhase2;
    [SerializeField] private GameObject landlord;
    [SerializeField] private GameObject villain;
    [SerializeField] private DoorWithKeyObject MainDoorL;
    [SerializeField] private DoorWithKeyObject MainDoorR;
    [SerializeField] private DoorWithKeyObject stairDoorR;
    [SerializeField] private DoorWithKeyObject stairDoorL;
    [SerializeField] bool isInteracted = false;
    [SerializeField] string key = "Main";

    [Header("Video Player")]
    public VideoPlayer videoPlayer;
    public VideoClip trueEndVideo;

    public event Action OnTransScene;
    private void TransScene()
    {
        Debug.Log("Next Scene");
        videoPlayer.clip = trueEndVideo;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
        OnTransScene?.Invoke();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        //SceneManager.LoadScene(sceneName);
        if (villain != null) villain.SetActive(true);
        if (landlord != null) landlord.SetActive(false);
        if (MainDoorL != null && MainDoorR != null)
        {
            MainDoorL.CloseDoor();
            MainDoorR.CloseDoor();
            MainDoorL.SetKeyID(key);
            MainDoorR.SetKeyID(key);
        }
        if (stairDoorR != null && stairDoorL != null)
        {
            stairDoorL.SetKeyID(String.Empty);
            stairDoorR.SetKeyID(String.Empty);
        }
        managerPhase2.SetActive(true);
        vp.enabled = false;
    }

    public override void Interact(Transform player)
    {
        if (!isInteracted)
        {
            StartCoroutine(PlayMonologue());
            isInteracted = true;
        }
        else return;
    }

    private IEnumerator PlayMonologue()
    {
        monologueManager = transform.Find("Monologue").gameObject.GetComponent<MonologueManager>();
        if (monologueManager != null)
        {
            Debug.Log("Not null");
            monologueManager.gameObject.SetActive(true);
            while (monologueManager != null && !monologueManager.isMonologueFinished())
            {
                yield return null;
            }
            Debug.Log("Monologue end.");
            TransScene();
        }
        yield break;
    }

}
