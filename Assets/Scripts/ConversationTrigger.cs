using SojaExiles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class ConversationTrigger : MonoBehaviour
{
    public GameObject Scripts;
    public DialogueBox dialogueBox;
    public DialogueData dialogueData;
    //public PlayerMovement playerMovement;
    //public LandlordMovement landlordMovement;
    public MouseLook mouselook;
    public Transform lookTarget;
    public bool isShowed = false;

    public event Action OnEnterTrigger;

    private void Start()
    {
        //dialogueBox = Scripts.GetComponent<DialogueBox>();
        //if (dialogueBox == null)
        //{
        //    Debug.LogError("DialogueBox component not found on " + Scripts.name);
        //}
        //if (mouselook == null)
        //{
        //    Debug.LogError("MouseLook component not assigned in ConversationTrigger.");
        //}
        //if (playerMovement == null)
        //{
        //    Debug.LogError("WorkerMovement component not assigned in ConversationTrigger.");
        //}
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isShowed && other.CompareTag("Player"))
        {
            Debug.Log("Trigger conversation");
            
            //Scripts.SetActive(true);

           
            StartCoroutine(Show());
            OnEnterTrigger?.Invoke();
            Debug.Log("Start conversation");
            isShowed = true;
        }
    }
    
    protected virtual IEnumerator Show()
    {
        if (lookTarget != null) DialogueManager.Instance.SetTargetDialogueBox(lookTarget);
        if (dialogueData != null) DialogueManager.Instance.ShowDialogueBox(dialogueData);
        yield break;
    }
}
