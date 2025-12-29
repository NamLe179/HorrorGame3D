using SojaExiles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class TheCoupleConversationTrigger : ConversationTrigger
{
    [SerializeField] private Transform newLookTarget;
    [SerializeField] private DialogueData newDialogueData;
    protected override IEnumerator Show()
    {

        if (lookTarget != null) DialogueManager.Instance.SetTargetDialogueBox(lookTarget);
        if (dialogueData != null) DialogueManager.Instance.ShowDialogueBox(dialogueData);

        while (DialogueManager.Instance != null && DialogueManager.Instance.DialogueUI.isFinished != true)
        {
            yield return null;
        }
        if (newLookTarget != null) DialogueManager.Instance.SetTargetDialogueBox(newLookTarget);
        if (newDialogueData != null) DialogueManager.Instance.ShowDialogueBox(newDialogueData);
        if (isShowed) Destroy(gameObject);
    }
    
}
