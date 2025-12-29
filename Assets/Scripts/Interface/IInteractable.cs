using UnityEngine;
using System.Collections;

namespace HorrorGame3D.Interaction
{
	public interface IInteractable
	{
		bool CanInteract();        
		void Interact(Transform player);
	}
}
