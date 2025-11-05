using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorrorGame3D.Interaction;

public class Item : MonoBehaviour, IInteractable
{
    [Header("Item Information")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [TextArea] [SerializeField] private string itemDescription;
    [SerializeField] public ItemOption.ItemType itemType = ItemOption.ItemType.Reuseable; // ⭐ Thêm ItemType
    [SerializeField] private ItemOption itemOption; // ⭐ Optional: Reference đến ScriptableObject nếu có
    
    [Header("Interaction Settings")]
    [SerializeField] private string collectPrompt = "Press E to collect";
    
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
            
            // Hủy object sau khi thu thập
            Destroy(gameObject);
            
            Debug.Log($"✅ Collected: {itemName} (Type: {itemType})");
        }
        else
        {
            Debug.LogError("❌ InventoryManager not found!");
        }
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
