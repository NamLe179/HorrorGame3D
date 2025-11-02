//using System.Collections;
//using UnityEngine;

//namespace HorrorGame3D.Interaction
//{
//	public class OvenInteract : InteractableBase
//	{
//		public Animator animator;

//		void Start()
//		{
//			if (animator == null)
//			{
//				animator = GetComponent<Animator>();
//			}
//		}

//		public override void Interact(Transform player)
//		{
//			if (!CanInteract())
//			{
//				return;
//			}

//			if (isOpen)
//				StartCoroutine(CloseDoor());
//			else
//				StartCoroutine(OpenDoor());
//		}

//		public override string GetPromptMessage()
//		{
//			if (isLocked)
//			{
//				if (requiredKey == null) return "🔒 Locked. Need a key.";
//				else return $"🔒 Locked. Need a {requiredKey.displayName}";
//			}

//			return isOpen ? "Press E to close" : "Press E to open";
//		}

//		private IEnumerator OpenDoor()
//		{
//			animator.Play("OpenOven");
//			isOpen = true;
//			yield return new WaitForSeconds(0.5f);
//		}

//		private IEnumerator CloseDoor()
//		{
//			animator.Play("ClosingOven");
//			isOpen = false;
//			yield return new WaitForSeconds(0.5f);
//		}
//	}
//}