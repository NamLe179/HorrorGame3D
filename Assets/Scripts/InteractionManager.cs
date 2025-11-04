using HorrorGame3D.Interaction;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
	[SerializeField] private Camera playerCamera;
	[SerializeField] private float maxRayDistance = 5f;
	[SerializeField] private LayerMask interactableLayer;
	[SerializeField] private Text promptText;
	[SerializeField] private KeyCode interactKey = KeyCode.E;

	private IInteractable current;

	void Update()
	{
		Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, interactableLayer))
		{   
			if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
			{
				current = interactable;
				promptText.gameObject.SetActive(true);
				promptText.text = interactable.GetInteractionText();

				if (Input.GetKeyDown(interactKey))
					interactable.Interact(gameObject);

				return;
			}
		}

		// Nếu không nhìn thấy object tương tác
		if (current != null)
		{
			current = null;
			promptText.gameObject.SetActive(false);
		}
	}
}
