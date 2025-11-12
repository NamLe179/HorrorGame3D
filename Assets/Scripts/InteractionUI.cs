using TMPro;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public class InteractionUI : MonoBehaviour
	{
		public static InteractionUI Instance;

		[SerializeField] private TextMeshProUGUI promptText;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);

			HidePrompt();
		}

		public void ShowPrompt(string message)
		{
			promptText.text = message;
			promptText.gameObject.SetActive(true);
		}

		public void HidePrompt()
		{
			promptText.gameObject.SetActive(false);
		}
	}
}

