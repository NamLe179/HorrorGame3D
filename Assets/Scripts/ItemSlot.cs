using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    // DATA
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;

    // SLOT
    [SerializeField] private Image itemImage;
    
    public void AddItem(string name, Sprite sprite)
    {
        this.itemName = name;
        this.itemSprite = sprite;
        isFull = true;

        itemImage.sprite = itemSprite;
        // itemImage.enabled = true;
    }
}
