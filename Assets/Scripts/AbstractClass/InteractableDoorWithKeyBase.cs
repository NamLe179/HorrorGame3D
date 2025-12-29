using HorrorGame3D.Interaction;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HorrorGame3D.Interaction
{
	public abstract class InteractableDoorWithKeyBase : InteractableDoorBase
	{
		[Header("Key Settings")]
        [SerializeField] protected string keyID;
        protected InventoryManager inventoryManager;
		[Header("Dialogue Settings")]
        [SerializeField] public DialogueData dialogueData;
        public event Action OnInteract;

		public void SetKeyID(string id)
		{
			keyID = id;
        }
        protected override void Awake()
		{
			base.Awake();

			inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
		}

		//protected override void Start()
		//{
		//	base.Start();

		//	//Nếu object bị thiếu component, ghi warning
		//	if (inventoryManager == null)
		//		Debug.LogWarning($"⚠️ InventoryManager NOT FOUND.");
		//}

		public override void Interact(Transform player)
		{
			if (!CanInteract())
			{
				return;
			}

			if (!isOpen)
			{
				if (!string.IsNullOrEmpty(keyID) && !inventoryManager.HasRequiredKey(keyID))
				{
					if (dialogueData != null) DialogueManager.Instance.ShowDialogueBox(dialogueData);
					
                    return;
				}
				OpenDoor();
				OnInteract?.Invoke();
            }
			else
			{
				CloseDoor();
            }
		}
	}
}

