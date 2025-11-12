using System.Collections;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public class SafeDoorInteract : InteractableBase
	{
		[Header("Door Settings")]
		[SerializeField] private Animator animator;  // Animator để mở cửa két
		[SerializeField] private AudioSource doorSound;  // Tiếng mở cửa
		[SerializeField] private float soundDelay = 0.1f;
		[SerializeField] private SafeLockMiniGame miniGame;

		void Start()
		{
			if (animator == null)
				animator = GetComponent<Animator>();

			if (doorSound == null)
				doorSound = GetComponent<AudioSource>();

		}

		public override void Interact(Transform player)
		{
			if (!CanInteract())
			{
				return;
			}

			if (isOpen)
			{
				StartCoroutine(CloseDoor());
			}
			else
			{
				miniGame.OnUnlock -= HandleUnlock; // gỡ cũ
				miniGame.OnUnlock += HandleUnlock; // đăng ký lại
				miniGame.StartMiniGame();
			}
		}

		public override string GetPromptMessage()
		{
			if (isLocked) return "🔒 The safe is locked...";
			return isOpen ? "Press E to close the safe" : "Press E to open the safe";
		}

		private IEnumerator PlaySoundWithDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			doorSound.Play();
		}

		private void HandleUnlock()
		{
			StartCoroutine(OpenDoor());
		}

		private IEnumerator OpenDoor()
		{
			animator.Play("open");
			isOpen = true;
			yield return new WaitForSeconds(0.5f);
		}

		private IEnumerator CloseDoor()
		{
			animator.Play("close");
			isOpen = false;
			yield return new WaitForSeconds(0.5f);
		}
	}
}
