using Controllers;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HorrorGame3D.Interaction
{
	public abstract class InteractSceneLoaderBase : InteractableBase
	{
		[SerializeField] protected string sceneName;

		protected override void Awake()
		{
			base.Awake();
			if (sceneName == null)
				Debug.LogWarning($"⚠️ {gameObject.name} does NOT have Scene Name.");

		}

		public override IEnumerator Open()
		{
            PauseGameInput.OnLoadMinigame?.Invoke();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			while (!asyncLoad.isDone)
				yield return null;

			

            PlayerMoveAndCameraController.DisableAll();
			yield return new WaitForSeconds(0.5f);
		}
	}
}
