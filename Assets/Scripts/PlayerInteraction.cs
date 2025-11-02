using HorrorGame3D.Interaction;
using UnityEngine;

namespace MyGame.Interaction
{
	public class PlayerInteraction : MonoBehaviour
	{
		public Camera playerCamera;
		public float interactRange = 0.5f;
		public KeyCode interactKey = KeyCode.E;

		void Update()
		{
			if (Input.GetKeyDown(interactKey))
			{
				Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
				int interactableMask = LayerMask.GetMask("Interactable");
				if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableMask))
				{
					IInteractable interactable = hit.collider.GetComponent<IInteractable>();
					if (interactable != null)
					{
						interactable.Interact(transform);
					}
				}
			}
		}
	}
}
