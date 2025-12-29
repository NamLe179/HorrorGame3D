using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonologueManager : MonoBehaviour
{

    [SerializeField] private DialogueBox dialogueBox;
    
    public bool isMonologueFinished()
    {
        return dialogueBox.isFinished;
    }
    private void Awake()
    {
        dialogueBox = GetComponent<DialogueBox>();
    }


}
