using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // DATA
    public string itemName;
    public Sprite itemSprite;
    public string itemDescription;
    public bool isFull;
    public Sprite emptySprite;
    public ItemOption itemOption; // Reference to ItemOption ScriptableObject

    // SLOT
    [SerializeField] private Image itemImage;

    // ITEM DESCRIPTION
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;
    private ItemOnHand itemOnHand;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        itemOnHand = GameObject.Find("OnHandSlot").GetComponent<ItemOnHand>();
    }

    public void AddItem(string name, Sprite sprite, string description, ItemOption option = null)
    {
        this.itemName = name;
        this.itemSprite = sprite;
        this.itemDescription = description;
        this.itemOption = option;
        isFull = true;

        itemImage.sprite = itemSprite;
        // itemImage.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    private void OnLeftClick()
    {
        // Nếu item đang được chọn (click lần 2) -> Sử dụng item
        if (thisItemSelected && isFull && itemOption != null)
        {
            UseItem();
            return;
        }

        // Click lần 1 -> Chọn item và hiển thị thông tin
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
        if(itemDescriptionImage == null)
        {
            itemDescriptionImage.sprite = emptySprite;
        }
    }

    private void UseItem()
    {
        if (itemOption == null) return;

        switch (itemOption.itemType)
        {
            case ItemOption.ItemType.Reuseable:
                // Item có thể tái sử dụng -> Không xóa khỏi inventory
                itemOption.UseItem();
                break;

            case ItemOption.ItemType.Consumable:
                // Item tiêu hao -> Xóa khỏi inventory
                itemOption.UseItem();
                EmptySlot();
                break;

            case ItemOption.ItemType.Equipment:
                // Item trang bị -> Đưa lên tay và XÓA khỏi inventory
                if (itemOnHand != null)
                {
                    itemOnHand.EquipItem(itemName, itemSprite, itemOption);
                    EmptySlot(); // ⭐ Xóa khỏi inventory để đảm bảo chỉ có 1 item duy nhất
                    Debug.Log($"📦 [ItemSlot] Equipment moved to hand, removed from inventory");
                }
                break;
        }

        // Deselect sau khi sử dụng
        inventoryManager.DeselectAllSlots();
    }

    private void EmptySlot()
    {
        itemName = "";
        itemSprite = emptySprite;
        itemDescription = "";
        itemOption = null;
        isFull = false;
        
        itemImage.sprite = emptySprite;
        thisItemSelected = false;
        selectedShader.SetActive(false);
        
        // Clear description UI
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
    }

    private void OnRightClick()
    {
        Debug.Log("Right Clicked on " + itemName);
    }
}
