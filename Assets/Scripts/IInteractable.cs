using UnityEngine;

public interface IInteractable
{
	string GetInteractionText();
	float GetInteractDistance();
	void Interact(GameObject interactor);
}
