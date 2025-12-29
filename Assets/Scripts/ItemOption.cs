using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemOption : ScriptableObject
{
	[Header("Item Info")]
	public string itemName;
	public string itemDescription;
	public Sprite itemSprite;
	public ItemType itemType;

	public void UseItem()
	{
		switch (itemType)
		{
			case ItemType.Reuseable:
				UseReuseable();
				break;

			case ItemType.Equipment:
				UseEquipment();
				break;

			case ItemType.Consumable:
				UseConsumable();
				break;
		}
	}

	private void UseReuseable()
	{
		Debug.Log($"✅ Using reusable item: {itemName}");
		// Logic cho item có thể tái sử dụng
		// Ví dụ: Flashlight, Key, Map, etc.

		// Thêm logic cụ thể ở đây
		// Ví dụ: Bật/tắt đèn pin, mở cửa, hiển thị bản đồ...
	}

	private void UseEquipment()
	{
		Debug.Log($"⚔️ Equipping item: {itemName}");
		// Logic cho equipment
		// Item sẽ được hiển thị trên tay qua ItemOnHand
		// Có thể thêm stats, abilities, etc.
	}

	private void UseConsumable()
	{
		Debug.Log($"🍖 Consuming item: {itemName}");
		// Logic cho item tiêu hao
		// Ví dụ: Hồi máu, hồi stamina, buff tạm thời...


	}

	public enum ItemType
	{
		Reuseable,    // Item có thể dùng nhiều lần, không mất (Key, Flashlight, Map...)
		Equipment,    // Item trang bị trên tay (Weapon, Tool...)
		Consumable    // Item tiêu hao, dùng 1 lần rồi mất (Health Potion, Food...)
	}
}
