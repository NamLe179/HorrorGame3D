using HorrorGame3D.Interaction;
using System;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public abstract class InteractableDoorWithPasswordBase : InteractableDoorBase
	{
		[SerializeField] protected String password;

		[SerializeField] protected DoorPasswordUI passwordUI;

        [SerializeField] public DialogueData dialogueData;

        private bool isWaitingForPassword = false;

		protected override void Awake()
		{
			base.Awake();

			if (passwordUI == null)
			{
				Debug.LogWarning("Password UI chưa assign!");
			}
		}

		public override void Interact(Transform player)
		{
			if (!CanInteract())
			{
				return;
			}

			if (!isOpen)
			{
				OpenPasswordUI();
			}
			else
			{
				CloseDoor();
            }
		}

		private void OpenPasswordUI()
		{
			isWaitingForPassword = true;
			passwordUI.ShowUI();
		}

		public void SubmitPassword(string input)
		{
			if (!isWaitingForPassword) return;

			if (input == password)
			{
				isWaitingForPassword = false;

				passwordUI.HideUI();

				OpenDoor(); // mở cửa
				if (dialogueData != null) DialogueManager.Instance.ShowDialogueBox(dialogueData);
            }
			else
			{
				passwordUI.ShowError("Wrong Password!!!");
			}
		}
	}

}
