using HorrorGame3D.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
    public abstract class ToggleInteractableBase : InteractableBase
    {
		[SerializeField] protected bool isOpen = false;

		public override void Interact(Transform player)
		{
			if (!CanInteract())
			{
				return;
			}

			if (!isOpen)
			{
				StartCoroutine(Open());
			}
			else 
			{
				StartCoroutine(Close());
			}
		}

		public override IEnumerator Open()
		{
			Debug.Log("Open");
			animator.Play("open");
			if (sound) sound.Play();
			yield return new WaitForSeconds(0.5f);
            isOpen = true;
        }

		public virtual IEnumerator Close()
		{
			animator.Play("close");
			if (sound) sound.Play();
			yield return new WaitForSeconds(0.5f);
            isOpen = false;
        }

	}

}
