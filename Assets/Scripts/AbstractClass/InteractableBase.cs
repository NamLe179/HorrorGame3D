using UnityEngine;
using System.Collections;

namespace HorrorGame3D.Interaction
{
	public abstract class InteractableBase : MonoBehaviour, IInteractable
	{
		[SerializeField] protected bool isLocked = false;

		[SerializeField] protected Animator animator;  

		[SerializeField] protected AudioSource sound; 
		
		[SerializeField] protected AudioClip clip;

		public abstract IEnumerator Open();

		protected virtual void Awake()
		{
			// Tự động gán Animator nếu có
			if (animator == null) 
			{ 
				animator = GetComponent<Animator>();
			}

			// Tự động gán AudioSource nếu có
			if (sound == null)
			{
				sound = GetComponent<AudioSource>();
			}
		}

		protected virtual void Start()
		{
			// Nếu object bị thiếu component, ghi warning
			//if (animator == null)
			//	Debug.LogWarning($"⚠️ {gameObject.name} does NOT have Animator.");

			//if (sound == null)
			//	Debug.LogWarning($"⚠️ {gameObject.name} does NOT have Sound.");
		}

		public virtual bool CanInteract()
		{
			// Nếu object bị khóa → không thể tương tác
			if (isLocked )
				return false;

			return true;
		}

		public virtual void Interact(Transform player)
		{
			if (!CanInteract())
			{
				return;
			}
            StartCoroutine(Open());
		}

	}
}
