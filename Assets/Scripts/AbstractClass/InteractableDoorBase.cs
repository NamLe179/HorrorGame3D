using HorrorGame3D.Interaction;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public abstract class InteractableDoorBase : ToggleInteractableBase
	{
		public bool IsOpen()
		{
			return isOpen;
		}

		public virtual void OpenDoor()
		{
			StartCoroutine(Open());
			PlayLockDoor();
        }

		public virtual void CloseDoor()
		{
			StartCoroutine(Close());
			PlayLockDoor();
        }

		public void PlayLockDoor()
        {
            if (sound != null && clip != null) sound.PlayOneShot(clip);
        }

        private void OnTriggerEnter(Collider other)
		{
			Debug.Log(other.name);
			if (other.gameObject.CompareTag("NPC") && !isOpen)
			{
				OpenDoor();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.CompareTag("NPC") && isOpen)
			{
				CloseDoor();
			}
		}
	}
}