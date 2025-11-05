using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller để xử lý việc sử dụng item khi đang cầm trên tay
/// </summary>
public class ItemUsageController : MonoBehaviour
{
    [Header("References")]
    public ItemOnHand itemOnHand;
    public ItemEquipmentManager equipmentManager;

    [Header("Light Settings")]
    public KeyCode usageKey = KeyCode.Mouse0; // Left Click
    private GameObject spotLightObject; // ⭐ Thay đổi: Lưu GameObject thay vì Light component
    private bool isLightOn = false;

    void Start()
    {
        itemOnHand = FindObjectOfType<ItemOnHand>();
        equipmentManager = FindObjectOfType<ItemEquipmentManager>();
    }

    void Update()
    {
        // Kiểm tra nếu đang cầm item và nhấn chuột trái
        if (Input.GetKeyDown(usageKey))
        {
            UseCurrentItem();
        }
    }

    private void UseCurrentItem()
    {
        // Kiểm tra có item trên tay không
        if (itemOnHand == null || !itemOnHand.hasEquippedItem)
        {
            return;
        }

        string itemName = itemOnHand.currentItemName.Trim();
        
        // Kiểm tra nếu là Phone hoặc Flashlight
        if (itemName == "Phone" || itemName == "Flashlight")
        {
            ToggleLight(itemName);
        }
    }

    private void ToggleLight(string itemName)
    {
        // Tìm Light component trong equipment hiện tại
        if (equipmentManager == null)
        {
            Debug.LogError("❌ [ItemUsageController] ItemEquipmentManager not found!");
            return;
        }

        GameObject currentEquipment = equipmentManager.GetCurrentEquipment();
        
        if (currentEquipment == null)
        {
            Debug.LogWarning("⚠️ [ItemUsageController] No equipment found!");
            return;
        }

        // ⭐ Tìm GameObject tên "SpotLight" (tìm cả inactive)
        if (spotLightObject == null)
        {
            // Tìm trong children
            Transform spotLightTransform = currentEquipment.transform.Find("SpotLight");
            
            if (spotLightTransform == null)
            {
                // Thử tìm trong tất cả children (nếu SpotLight là grand child)
                foreach (Transform child in currentEquipment.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "SpotLight")
                    {
                        spotLightTransform = child;
                        break;
                    }
                }
            }
            
            if (spotLightTransform != null)
            {
                spotLightObject = spotLightTransform.gameObject;
                Debug.Log($"✅ [ItemUsageController] Found SpotLight GameObject on: {currentEquipment.name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ [ItemUsageController] No 'SpotLight' GameObject found on {itemName}!");
                
                // Debug: Liệt kê tất cả children
                Debug.LogWarning($"🔍 Available children:");
                foreach (Transform child in currentEquipment.GetComponentsInChildren<Transform>(true))
                {
                    Debug.LogWarning($"   - {child.name}");
                }
                return;
            }
        }

        if (spotLightObject != null)
        {
            // ⭐ Toggle GameObject SpotLight (bật/tắt cả GameObject)
            isLightOn = !isLightOn;
            spotLightObject.SetActive(isLightOn);

            string status = isLightOn ? "ON 💡" : "OFF 🔦";
            Debug.Log($"🔦 [{itemName}] SpotLight {status}");
        }
        else
        {
            Debug.LogWarning($"⚠️ [ItemUsageController] No SpotLight GameObject found on {itemName}!");
        }
    }

    // Reset khi unequip item
    public void OnItemUnequipped()
    {
        spotLightObject = null;
        isLightOn = false;
    }

    // Set light khi equip item mới
    public void OnItemEquipped()
    {
        spotLightObject = null;
        isLightOn = false;
    }
}
