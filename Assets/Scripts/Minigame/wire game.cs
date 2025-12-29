using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main Instance;

    public int switchCount;
    public GameObject winText;
    private int onCount = 0;

    private void Awake()
    {
        Instance = this;
    }
    public void SwitchChange(int points) {
        onCount = onCount + points;
        if (onCount == switchCount)
        {
            //winText.SetActive(true);
			CutSceneLoader.endingID = 2;
			SceneManager.LoadScene("CutScenePlayer");
        }
		
	}
	public void UnloadAll2DScenes()
	{
		for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
		{
			Scene s = SceneManager.GetSceneAt(i);

			if (s.name == "BackgroundScene")
				continue;

			if (s.name == "DontDestroyOnLoad")
				continue;

			Debug.Log("Unloading 2D scene: " + s.name);
			SceneManager.UnloadSceneAsync(s);
		}
	}
	private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
			UnloadAll2DScenes();
			PlayerMoveAndCameraController.EnableAll();
		}
    }
}
