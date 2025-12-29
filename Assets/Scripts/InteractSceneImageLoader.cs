using HorrorGame3D.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractSceneImageLoader : InteractSceneLoaderObject
{
    [SerializeField] private MonologueManager monologueManagerFull;
    [SerializeField] private MonologueManager monologueManagerHalf;
    [SerializeField] private MonologueManager monologueManagerGameCompleted;
    [Header("Dialogue Data")]
    [SerializeField] private DialogueData dialogueDataFull;
    [SerializeField] private DialogueData dialogueDataHalf;
    [SerializeField] private DialogueData dialogueDataGameCompleted;

    [SerializeField] private GameObject image;
    [SerializeField] private GameObject brainJar;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    protected InventoryManager inventoryManager;
    protected override void Awake()
    {
        base.Awake();

        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        if (inventoryManager == null) Debug.LogWarning($"InventoryManager NOT FOUND.");
        monologueManagerHalf = transform.Find("MonologueHalf").gameObject.GetComponent<MonologueManager>();
        monologueManagerFull = transform.Find("MonologueFull").gameObject.GetComponent<MonologueManager>();
        monologueManagerGameCompleted = transform.Find("MonologueGameCompleted").gameObject.GetComponent<MonologueManager>();
        image = transform.Find("Complete_frame").gameObject;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public override void Interact(Transform player)
    {
        if (inventoryManager != null)
        {
            var state = inventoryManager.hasItemImageState();
            switch (state)
            {
                case InventoryManager.ImageState.Half:
                    Debug.Log("Half state");
                    //StartCoroutine(PlayMonologue());
                    StartCoroutine(PlayMonologue(dialogueDataHalf));
                    break;
                case InventoryManager.ImageState.Full:
                    Debug.Log("Full state");
                    StartCoroutine(HandleOpenScene());
                    break;
            }

        }
    }

    private IEnumerator PlayMonologue()
    {
        //if (monologueManagerHalf==null)  monologueManagerHalf = transform.Find("MonologueHalf").gameObject.GetComponent<MonologueManager>();
        //if (monologueManagerHalf != null)
        //{
        //    Debug.Log("Not null");
        //    monologueManagerHalf.gameObject.SetActive(true);
        //    while (monologueManagerHalf != null && !monologueManagerHalf.isMonologueFinished())
        //    {
        //        yield return null;
        //    }
        //    Debug.Log("Monologue end.");
        //}

        if (dialogueDataHalf != null)
        {
            DialogueManager.Instance.ShowDialogueBox(dialogueDataHalf);
            while (DialogueManager.Instance != null && DialogueManager.Instance.DialogueUI.isFinished != true)
            {
                yield return null;
            }
        }

        //yield break;
    }

    private IEnumerator PlayMonologue(DialogueData dialogueData)
    {
        if (dialogueData != null)
        {
            DialogueManager.Instance.ShowDialogueBox(dialogueData);
            while (DialogueManager.Instance != null && DialogueManager.Instance.DialogueUI.isFinished != true)
            {
                yield return null;
            }
        }

    }


    // event 
    private IEnumerator HandleOpenScene()
    {
        //monologueManagerFull = transform.Find("MonologueFull").gameObject.GetComponent<MonologueManager>();
        //if (monologueManagerFull != null)
        //{
        //    Debug.Log("Not null");
        //    monologueManagerFull.gameObject.SetActive(true);

        //    while (monologueManagerFull != null && !monologueManagerFull.isMonologueFinished())
        //    {
        //        yield return null;
        //    }
        //    Debug.Log("Monologue end.");
        //}

        //if (dialogueDataFull != null)
        //{
        //    DialogueManager.Instance.ShowDialogueBox(dialogueDataFull);
        //    while (DialogueManager.Instance != null && DialogueManager.Instance.DialogueUI.isFinished != true)
        //    {
        //        yield return null;
        //    }
        //}

        yield return PlayMonologue(dialogueDataFull);

        yield return StartCoroutine(Open());

        PauseGameInput.OnUnloadMinigame?.Invoke();

        ActiveTheImage();
        ActiveTheBrainJar();

        yield return PlayMonologue(dialogueDataGameCompleted);
    }
    public override IEnumerator Open()
    {
        // Check Game completed or not
        if (sceneName == "game_puzzle" && PuzzleGameManager.gameCompleted)
        {
            yield break;
        }
        // Not completed -> Open scene as usual
        yield return base.Open();

        // Wait to Scene game unloaded
        if (sceneName == "game_puzzle")
        {
            yield return new WaitUntil(() => !SceneManager.GetSceneByName(sceneName).isLoaded);
        }

        //if (image != null && PuzzleGameManager.gameCompleted) image.SetActive(true);
        //if (monologueManagerGameCompleted != null && PuzzleGameManager.gameCompleted)
        //{
        //    monologueManagerGameCompleted.gameObject.SetActive(true);
        //    while (monologueManagerGameCompleted != null && !monologueManagerGameCompleted.isMonologueFinished())
        //    {
        //        yield return null;
        //    }
            
        //}
    }


    private void ActiveTheImage()
    {
        if (image != null && PuzzleGameManager.gameCompleted) image.SetActive(true);
    }

    private void ActiveTheBrainJar()
    {
        if (brainJar != null)
        {
            brainJar.SetActive(true);
            brainJar.GetComponent<InteractionObjectTransScene>().OnTransScene += StopSound;
            if (audioSource != null && audioClip != null) audioSource.PlayOneShot(audioClip);
        }
    }

    private void StopSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
