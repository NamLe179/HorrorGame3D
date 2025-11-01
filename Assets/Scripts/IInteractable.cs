using UnityEngine;

namespace HorrorGame3D.Interaction
{
	public interface IInteractable
	{
		bool CanInteract();        
		void Interact(Transform player);
		string GetPromptMessage(); 
	}
}
