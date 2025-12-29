using System;
using System.Collections;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public class SafeDoorInteract : ToggleInteractableBase
	{
		[Header("Door Settings")]
		[SerializeField] private SafeLockMiniGame miniGame;
        [SerializeField] private MonologueManager monologueManager;

        bool hasPlayedMonologue = false;


        private IEnumerator OpenMinigame()
        {
			if (monologueManager != null && !hasPlayedMonologue)
			{
				Debug.Log("Not null");
				monologueManager.gameObject.SetActive(true);

				while (monologueManager != null && !monologueManager.isMonologueFinished())
				{
					yield return null;
				}
				hasPlayedMonologue = true;
				Debug.Log("Monologue end.");
			}


			miniGame.OnUnlock -= HandleUnlock; // gỡ cũ
            miniGame.OnUnlock += HandleUnlock; // đăng ký lại
			miniGame.StartMiniGame();
			yield break;
        }


        public override void Interact(Transform player)
		{
			if (!CanInteract())
			{
				return;
			}

			if (isOpen)
			{
				StartCoroutine(Close());
			}
			else
			{
				StartCoroutine(OpenMinigame());
            }
		}

		private void HandleUnlock()
		{
			StartCoroutine(Open());
		}

		public override IEnumerator Open()
		{
			animator.Play("open");
			if (sound) sound.Play();
			isOpen = true;
			yield return new WaitForSeconds(0.5f);
		}

		public override IEnumerator Close()
		{
			animator.Play("close");
			if (sound) sound.Play();
			isOpen = false;
			yield return new WaitForSeconds(0.5f);
		}
	}
}
