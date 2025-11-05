using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemOnHand : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public Image onHandSlotImage;
    public Sprite emptySprite;

    [Header("Current Equipped Item")]
    public string currentItemName;
    public Sprite currentItemSprite;
    public ItemOption currentItemOption;
    public bool hasEquippedItem = false;

    [Header("3D Item Display")]
    public Transform handTransform; // Vị trí tay người chơi
    public ItemEquipmentManager equipmentManager;

    public InventoryManager inventoryManager;
    public ItemUsageController usageController;

    void Start()
    {
        Debug.Log("🔧 [ItemOnHand] Start() called");
        
        // Find InventoryManager
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("❌ [ItemOnHand] InventoryManager NOT FOUND!");
        }

        // Find ItemUsageController
        usageController = FindObjectOfType<ItemUsageController>();
        if (usageController == null)
        {
            Debug.LogWarning("⚠️ [ItemOnHand] ItemUsageController not found");
        }
        
        if (equipmentManager == null)
        {
            Debug.LogWarning("⚠️ [ItemOnHand] equipmentManager is NULL, trying to find...");
            equipmentManager = FindObjectOfType<ItemEquipmentManager>();
        }
        
        if (equipmentManager != null)
        {
            Debug.Log($"✅ [ItemOnHand] equipmentManager found: {equipmentManager.gameObject.name}");
        }
        else
        {
            Debug.LogError("❌ [ItemOnHand] equipmentManager NOT FOUND!");
        }

        if (handTransform != null)
        {
            Debug.Log($"✅ [ItemOnHand] handTransform assigned: {handTransform.name} at position {handTransform.position}");
        }
        else
        {
            Debug.LogError("❌ [ItemOnHand] handTransform is NULL!");
        }
        
        ClearSlot();
    }

    public void EquipItem(string itemName, Sprite itemSprite, ItemOption itemOption)
    {
        Debug.Log($"🔧 [ItemOnHand] EquipItem called: '{itemName}'");
        
        // ⭐ Nếu đã có item trên tay, TRẢ VỀ inventory trước khi unequip
        if (hasEquippedItem && inventoryManager != null)
        {
            Debug.Log($"📦 [ItemOnHand] Returning current item to inventory: {currentItemName}");
            inventoryManager.AddItem(currentItemName, currentItemSprite, currentItemOption.itemDescription, currentItemOption);
        }
        
        // Unequip item cũ (ẩn 3D model và notify usage controller)
        if (hasEquippedItem)
        {
            // Notify usage controller trước
            if (usageController != null)
            {
                usageController.OnItemUnequipped();
            }
            
            // Ẩn 3D model
            if (equipmentManager != null)
            {
                equipmentManager.HideEquipment();
            }
        }

        // Equip item mới
        currentItemName = itemName;
        currentItemSprite = itemSprite;
        currentItemOption = itemOption;
        hasEquippedItem = true;

        // Cập nhật UI slot
        if (onHandSlotImage != null)
        {
            onHandSlotImage.sprite = itemSprite;
            Debug.Log($"✅ [ItemOnHand] UI sprite updated");
        }
        else
        {
            Debug.LogError("❌ [ItemOnHand] onHandSlotImage is NULL!");
        }

        // Hiển thị item 3D trên tay
        if (equipmentManager != null && handTransform != null)
        {
            Debug.Log($"🔧 [ItemOnHand] Calling ShowEquipment('{itemName}', handTransform at {handTransform.position})");
            equipmentManager.ShowEquipment(itemName, handTransform);
        }
        else
        {
            Debug.LogError($"❌ [ItemOnHand] Cannot show equipment!");
            Debug.LogError($"   equipmentManager: {(equipmentManager != null ? "✅" : "❌ NULL")}");
            Debug.LogError($"   handTransform: {(handTransform != null ? "✅" : "❌ NULL")}");
        }

        // Notify usage controller
        if (usageController != null)
        {
            usageController.OnItemEquipped();
        }

        Debug.Log($"✅ [ItemOnHand] Equipped: {itemName}");
    }

    public void UnequipItem()
    {
        if (!hasEquippedItem) return;

        Debug.Log($"⬇️ [ItemOnHand] Unequipped: {currentItemName}");

        // Notify usage controller
        if (usageController != null)
        {
            usageController.OnItemUnequipped();
        }

        // Ẩn item 3D
        if (equipmentManager != null)
        {
            equipmentManager.HideEquipment();
        }

        // Clear data
        ClearSlot();
    }

    private void ClearSlot()
    {
        currentItemName = "";
        currentItemSprite = emptySprite;
        currentItemOption = null;
        hasEquippedItem = false;

        if (onHandSlotImage != null)
        {
            onHandSlotImage.sprite = emptySprite;
        }
    }

    void Update()
    {
        // Có thể thêm logic để unequip bằng phím tắt
        // Ví dụ: nhấn G để bỏ item xuống
        if (Input.GetKeyDown(KeyCode.G) && hasEquippedItem)
        {
            UnequipItem();
        }
    }

    // ⭐ Implement IPointerClickHandler để bắt Right-Click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    private void OnRightClick()
    {
        if (hasEquippedItem)
        {
            Debug.Log($"🖱️ [ItemOnHand] Right-clicked, unequipping: {currentItemName}");
            
            // Trả item về inventory trước khi unequip
            if (inventoryManager != null)
            {
                inventoryManager.AddItem(currentItemName, currentItemSprite, currentItemOption.itemDescription, currentItemOption);
                Debug.Log($"📦 [ItemOnHand] Item returned to inventory: {currentItemName}");
            }
            
            UnequipItem();
        }
    }
}

