using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
	[SerializeField] private string interactionText = "Press E";
	[SerializeField] private float interactDistance = 1f;

	protected bool isActive = false; // (mở / tắt)

	public virtual string GetInteractionText() => interactionText;
	public virtual float GetInteractDistance() => interactDistance;

	public abstract void Interact(GameObject interactor);
}
