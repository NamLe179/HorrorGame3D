using HorrorGame3D.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractSceneLoaderMinigame : InteractSceneLoaderBase
{
    public event Func<IEnumerator> OnBefore;

    public override IEnumerator Open()
    {
        Debug.Log("InteractSceneLoaderMinigame: Open called.");
        if (OnBefore != null)
        {
            foreach (Delegate sub in OnBefore.GetInvocationList())
            {
                Func<IEnumerator> handler = (Func<IEnumerator>)sub;
                Debug.Log(handler.Method.Name);
                yield return handler();
            }
        }
        yield return base.Open();
        if (sceneName == "Game 2")
        {
            yield return new WaitUntil(() => !SceneManager.GetSceneByName(sceneName).isLoaded);
        }
        PauseGameInput.OnUnloadMinigame?.Invoke();
    }
}
