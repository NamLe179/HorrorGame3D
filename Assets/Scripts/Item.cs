using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorrorGame3D.Interaction;

public class Item : MonoBehaviour, IInteractable
{
	[Header("Item Information")]
	[SerializeField] private string itemName;
	[SerializeField] private Sprite itemSprite;
	[TextArea][SerializeField] private string itemDescription;
	[SerializeField] public ItemOption.ItemType itemType = ItemOption.ItemType.Reuseable; // ⭐ Thêm ItemType
	[SerializeField] private ItemOption itemOption; // ⭐ Optional: Reference đến ScriptableObject nếu có

	[Header("Interaction Settings")]
	[SerializeField] private string collectPrompt = "Press E to collect";
	[SerializeField] private MonologueManager monologueManager;
	[SerializeField] private DialogueData dialogueData;

    private InventoryManager inventoryManager;
	private const string INTERACTABLE_LAYER = "Interactable";

	void Start()
	{
		inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();


		// Gán layer Interactable cho item
		SetLayerRecursively(gameObject, INTERACTABLE_LAYER);
	}

	// Implement IInteractable
	public void Interact(Transform player)
	{
		if (inventoryManager != null)
		{
			// Tạo ItemOption dynamically nếu chưa có
			ItemOption option = itemOption;
			if (option == null)
			{
				option = ScriptableObject.CreateInstance<ItemOption>();
				option.itemName = itemName;
				option.itemDescription = itemDescription;
				option.itemSprite = itemSprite;
				option.itemType = itemType;
			}

			// Thêm item vào inventory với đầy đủ thông tin
			inventoryManager.AddItem(itemName, itemSprite, itemDescription, option);
            
			monologueManager = transform.GetComponentInChildren<MonologueManager>(true);
			if (dialogueData != null)
			{
				DialogueManager.Instance.ShowDialogueBox(dialogueData);
            }

			if (monologueManager != null)
			{
				monologueManager.gameObject.SetActive(true);

				Debug.Log("Starting monologue for collected item...");

				Collider col = GetComponent<Collider>();

				if (col) col.enabled = false;

				
			}
			else
			{
				Debug.Log("No MonologueManager found on this item.");
			}

            StartCoroutine(WaitAndDestroy());

            Debug.Log($"✅ Collected: {itemName} (Type: {itemType})");
		}
		else
		{
			Debug.LogError("❌ InventoryManager not found!");
		}
	}

	private IEnumerator WaitAndDestroy()
	{
        while (monologueManager != null && !monologueManager.isMonologueFinished())
        {
            yield return null;
        }

        Debug.Log("Destroying item after monologue...");
		Destroy(gameObject);
    }

    public bool CanInteract()
	{
		// Item luôn có thể thu thập
		return true;
	}

	public string GetPromptMessage()
	{
		return collectPrompt;
	}

	public string GetInteractionText()
	{
		return itemDescription;
	}

	public void Interact(GameObject interactor)
	{
		Interact(interactor.transform);
	}

	// Gán layer cho object và toàn bộ con của nó
	private void SetLayerRecursively(GameObject obj, string layerName)
	{
		int layer = LayerMask.NameToLayer(layerName);

		if (layer == -1)
		{
			Debug.LogWarning($"⚠️ Layer '{layerName}' Not Found! Please add layer in Project Settings > Tags and Layers.");
			return;
		}

		obj.layer = layer;

		foreach (Transform child in obj.transform)
		{
			SetLayerRecursively(child.gameObject, layerName);
		}
	}
}