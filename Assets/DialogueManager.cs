using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public DialogueBox DialogueUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DialogueUI = GetComponentInChildren<DialogueBox>(true);
    }

    public void ShowDialogueBox(DialogueData data)
    {
        Debug.Log("Show Dialogue Box");
        DialogueUI.dialogueLines = new List<DialogueLine>(data.dialogueLines);

        DialogueUI.gameObject.SetActive(true);
    }

    public void SetTargetDialogueBox(Transform target)
    {
        DialogueUI.target = target;
    }

    public void ResetDialogueBox()
    {
        this.DialogueUI.isFinished = false;
    }
}
