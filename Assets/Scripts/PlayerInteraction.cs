using HorrorGame3D.Interaction;
using UnityEngine;

namespace MyGame.Interaction
{
	public class PlayerInteraction : MonoBehaviour
	{
		public Camera playerCamera;
		public float interactRange = 1.5f;
		public KeyCode interactKey = KeyCode.E;

		void Update()
		{
			Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

			if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
			{
				IInteractable interactable = hit.collider.GetComponent<IInteractable>();

				if (interactable != null && interactable.CanInteract())
				{
                    if (InteractionUI.Instance != null) InteractionUI.Instance.ShowPrompt();

					if (Input.GetKeyDown(interactKey))
					{
						interactable.Interact(transform);

					}

					return;
				}
				else
				{
                    if (InteractionUI.Instance != null) InteractionUI.Instance.HidePrompt();
				}
			}
			else
			{
				if (InteractionUI.Instance != null)
                    InteractionUI.Instance.HidePrompt();
            }
        }
	}
}
