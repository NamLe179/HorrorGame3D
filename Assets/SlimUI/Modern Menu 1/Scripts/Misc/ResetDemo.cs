using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu{
	public class ResetDemo : MonoBehaviour {

		void Update () {
			if(Input.GetKeyDown("r")){
				Debug.Log("R is pressed!");
				SceneManager.LoadScene(0);
			}
		}
	}
}