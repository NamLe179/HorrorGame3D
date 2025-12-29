using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HorrorGame3D.Interaction
{
	public class DoorPasswordUI : MonoBehaviour
	{
		[Header("UI References")]
		[SerializeField] private GameObject passwordPanel;
		[SerializeField] private TMP_InputField passwordInput;
		[SerializeField] private TextMeshProUGUI errorText;
		[SerializeField] private Button confirmButton;
		[SerializeField] private Button quitButton;
		[SerializeField] private DoorWithPasswordObject door;
		private void Awake()
		{
			passwordPanel.SetActive(false);
			confirmButton.onClick.AddListener(OnConfirm);
			quitButton.onClick.AddListener(OnQuit);
        }

		public void ResetUI()
		{
			passwordInput.text = "";
			errorText.text = "";
		}

		public void ShowUI()
		{
			PlayerMoveAndCameraController.SetPlayerMovement(false);
			PlayerMoveAndCameraController.SetPlayerUI(false);
			PlayerMoveAndCameraController.SetPlayerCameraMove(false);

            ResetUI();
			passwordPanel.SetActive(true);
		}

		public void HideUI()
		{
			PlayerMoveAndCameraController.SetPlayerMovement(true);
			PlayerMoveAndCameraController.SetPlayerUI(true);
			PlayerMoveAndCameraController.SetPlayerCameraMove(true);
			ResetUI();
            passwordPanel.SetActive(false);
		}

		private void OnConfirm()
		{
			door.SubmitPassword(passwordInput.text);
		}

		public void ShowError(string msg)
		{
			errorText.text = msg;
		}

		private void OnQuit()
		{
			HideUI();
        }
	}

}
