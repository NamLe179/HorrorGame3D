using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipmentManager : MonoBehaviour
{
    [Header("Equipment Prefabs")]
    public List<EquipmentPrefab> equipmentPrefabs = new List<EquipmentPrefab>();

    private GameObject currentEquipmentObject;

    [System.Serializable]
    public class EquipmentPrefab
    {
        public string itemName;
        public GameObject prefab;
        
        [Header("Transform Offset")]
        public Vector3 positionOffset = Vector3.zero;
        public Vector3 rotationOffset = Vector3.zero; // ⭐ NEW: Rotation cho từng item
        public Vector3 scaleOffset = Vector3.one;
    }

    void Start()
    {
        Debug.Log($"🔧 [ItemEquipmentManager] Start() - Found {equipmentPrefabs.Count} equipment prefabs");
        for (int i = 0; i < equipmentPrefabs.Count; i++)
        {
            Debug.Log($"   [{i}] Name: '{equipmentPrefabs[i].itemName}' - Prefab: {equipmentPrefabs[i].prefab}");
        }
    }

    /// <summary>
    /// Hiển thị equipment 3D trên tay người chơi
    /// </summary>
    public void ShowEquipment(string itemName, Transform handTransform)
    {
        // Trim khoảng trắng
        string cleanName = itemName.Trim();
        
        Debug.Log($"🔧 [ItemEquipmentManager] ShowEquipment called");
        Debug.Log($"   Looking for: '{cleanName}'");
        
        // Ẩn equipment hiện tại nếu có
        HideEquipment();

        // Tìm prefab tương ứng với itemName
        EquipmentPrefab equipment = equipmentPrefabs.Find(e => e.itemName.Trim() == cleanName);

        if (equipment != null && equipment.prefab != null && handTransform != null)
        {
            Debug.Log($"✅ [ItemEquipmentManager] Found equipment, spawning...");
            
            // Instantiate prefab tại vị trí tay
            currentEquipmentObject = Instantiate(equipment.prefab, handTransform);
            
            // Apply offset từ settings
            currentEquipmentObject.transform.localPosition = equipment.positionOffset;
            currentEquipmentObject.transform.localRotation = Quaternion.Euler(equipment.rotationOffset); // ⭐
            currentEquipmentObject.transform.localScale = equipment.scaleOffset;

            Debug.Log($"✅ [ItemEquipmentManager] Equipment spawned: {currentEquipmentObject.name}");
            Debug.Log($"   Position: {equipment.positionOffset}");
            Debug.Log($"   Rotation: {equipment.rotationOffset}");
            Debug.Log($"   Scale: {equipment.scaleOffset}");
        }
        else
        {
            Debug.LogError($"❌ [ItemEquipmentManager] Equipment NOT FOUND for: '{cleanName}'");
        }
    }

    /// <summary>
    /// Ẩn equipment đang hiển thị
    /// </summary>
    public void HideEquipment()
    {
        if (currentEquipmentObject != null)
        {
            Debug.Log($"🗑️ [ItemEquipmentManager] Destroying equipment: {currentEquipmentObject.name}");
            Destroy(currentEquipmentObject);
            currentEquipmentObject = null;
        }
    }

    /// <summary>
    /// Thêm equipment prefab vào danh sách (có thể gọi từ editor hoặc code)
    /// </summary>
    public void AddEquipmentPrefab(string itemName, GameObject prefab)
    {
        if (equipmentPrefabs.Find(e => e.itemName == itemName) == null)
        {
            equipmentPrefabs.Add(new EquipmentPrefab { itemName = itemName, prefab = prefab });
            Debug.Log($"➕ [ItemEquipmentManager] Added equipment: '{itemName}'");
        }
    }

    /// <summary>
    /// Lấy equipment object hiện tại đang cầm
    /// </summary>
    public GameObject GetCurrentEquipment()
    {
        return currentEquipmentObject;
    }
}
