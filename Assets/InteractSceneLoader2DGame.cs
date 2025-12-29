using HorrorGame3D.Interaction;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSceneLoader2DGame : InteractSceneLoaderObject
{
    public event Func<IEnumerator> OnBefore;
    public event Action OnAfter;

    public override IEnumerator Open()
    {
        Debug.Log("InteractSceneLoaderMinigame: Open called.");
        yield return OnBefore?.Invoke();
        yield return base.Open();
        yield return new WaitUntil(() => GameManager.Instance.Finished);
        PauseGameInput.OnUnloadMinigame?.Invoke();
        OnAfter?.Invoke();
        GameManager.Instance.RestartGame();
    }
}
