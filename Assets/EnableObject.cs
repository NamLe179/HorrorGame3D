using HorrorGame3D.Interaction;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class EnableObject : MonoBehaviour
{
    [SerializeField] private SafeLockMiniGame miniGame;
    [SerializeField] private InteractSceneLoaderMinigame interactSceneLoaderMinigame;
    [SerializeField] private InteractSceneLoader2DGame interactSceneLoader2DGame;
    [SerializeField] private DialogueData before;
    [SerializeField] private DialogueData after;
    void Start()
    {
        if (miniGame != null)
        {
            Debug.Log("EnableObject: Subscribing to MiniGame OnUnlock event.");
            miniGame.OnUnlock += MiniGame_ShowAfter;
        }
        if (interactSceneLoaderMinigame != null)
        {
            Debug.Log("EnableObject: Subscribing to InteractSceneLoaderMinigame OnBefore event.");
            interactSceneLoaderMinigame.OnBefore += MiniGame_ShowBefore;
        }
        if (interactSceneLoader2DGame != null)
        {
            Debug.Log("EnableObject: Subscribing to InteractSceneLoader2DGame OnBefore event.");
            interactSceneLoader2DGame.OnBefore += MiniGame_ShowBefore;
            interactSceneLoader2DGame.OnAfter += MiniGame_ShowAfter;
        }
    }

    private IEnumerator MiniGame_ShowBefore()
    {
        if (before != null)
        {
            Show(before);
            yield return new WaitUntil(() => DialogueManager.Instance.DialogueUI.isFinished);
        }
    }
    private void MiniGame_ShowAfter()
    {
        Debug.Log("EnableObject: Activating object.");
        if (after != null)
        {
            Show(after);
        }
    }

    private void Show(DialogueData data)
    {
        DialogueManager.Instance.ShowDialogueBox(data);
    }

    //private IEnumerator WaitToFinished(DialogueBox dialogueBox)
    //{
        
    //    while (dialogueBox != null && !dialogueBox.isFinished)
    //    {
    //        yield return null;
    //    }
    //}
}
