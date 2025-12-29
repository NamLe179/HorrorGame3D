using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvents
{
    public static event Action<DialogueBox> OnDialogueLine;
    public static event Action<DialogueBox> OnDialogueStart;

    public static void RaiseDialogueStart(DialogueBox data)
    {
        OnDialogueStart?.Invoke(data);
    }

    public static void RaiseDialogueLine(DialogueBox line)
    {
        OnDialogueLine?.Invoke(line);
    }
}
