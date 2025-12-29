using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingTriggerZone : MonoBehaviour
{
	[Header("Cài đặt Ending")]
	public int endingIDToLoad; // Nhập số ID (1 hoặc 3) vào đây
	public string endingSceneName = "CutScenePlayer"; 

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			LoadEnding();
		}
	}

	void LoadEnding()
	{
		CutSceneLoader.endingID = endingIDToLoad;

		SceneManager.LoadScene(endingSceneName);
	}
}