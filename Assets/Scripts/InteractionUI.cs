using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HorrorGame3D.Interaction
{
	public class InteractionUI : MonoBehaviour
	{
		public static InteractionUI Instance;

		[SerializeField] private TextMeshProUGUI promptText;
		[SerializeField] private RawImage crossHair;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
            else
			{
				Destroy(gameObject);
			}

			HidePrompt();
			Debug.Log("[InteractionUI] Instance initialized.");
		}


		public void ShowPrompt()
		{
			crossHair.gameObject.SetActive(false);
			promptText.gameObject.SetActive(true);
		}

		public void HidePrompt()
		{
			crossHair.gameObject.SetActive(true);
			promptText.gameObject.SetActive(false);
		}
	}
}

