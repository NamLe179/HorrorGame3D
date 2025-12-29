using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
	public GameObject inventoryMenu;
	private bool menuActivated = false;
	public KeyCode keyCode = KeyCode.I;
	public ItemSlot[] itemSlot;

	private List<string> itemRequireNames = new List<string>()
	{
		"Part_1",
		"Part_2"
    };

    public enum ImageState
	{
		Half,
		Full
    }
    void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(keyCode))
		{
			menuActivated = !menuActivated;
			inventoryMenu.SetActive(menuActivated);
			if (menuActivated)
			{
				// Show cursor
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				Time.timeScale = 0f;
			}
			else
			{
				// Hide cursor
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				Time.timeScale = 1f;
				DeselectAllSlots();
			}
		}
	}
	public void AddItem(string itemName, Sprite itemSprite, string itemDescription, ItemOption itemOption = null)
	{
		for (int i = 0; i < itemSlot.Length; i++)
		{
			if (itemSlot[i].isFull == false)
			{
				itemSlot[i].AddItem(itemName, itemSprite, itemDescription, itemOption);
				return;
			}
		}
	}

	public void DeselectAllSlots()
	{
		for (int i = 0; i < itemSlot.Length; i++)
		{
			itemSlot[i].selectedShader.SetActive(false);
			itemSlot[i].thisItemSelected = false;
		}
	}

	public bool HasRequiredKey(string keyID)
	{
		// Duyệt tất cả slot trong inventory
		foreach (var slot in itemSlot)
		{
			if (slot.isFull && slot.itemOption != null)
			{
				// So sánh theo itemName
				if (slot.itemOption.itemName == keyID)
				{
					Debug.Log($"✅ Found key: {slot.itemOption.itemName}");
					return true;
				}
			}
		}

		return false;
	}

	public ImageState hasItemImageState()
	{
		int count = 0;
		foreach (var slot in itemSlot)
		{
			if (slot.isFull && slot.itemOption != null)
			{
				foreach (var requireName in itemRequireNames)
				{
                    if (requireName == slot.itemOption.itemName) {
						count++;
						break;
					}
                }
            }
				
        }
		if (count == itemRequireNames.Count) return ImageState.Full;
		else return ImageState.Half;
    }

}