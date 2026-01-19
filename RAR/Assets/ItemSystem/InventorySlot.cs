using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int itemCount;
    public ItemData ItemData => itemData;
    [SerializeField]public int ItemCount => itemCount;
    public InventorySlot(ItemData sourceItemData, int amount)
    {
        this.itemData = sourceItemData;
        this.itemCount = amount;
    }
    public InventorySlot()
    {
        this.itemData = null;
        this.itemCount = -1;
    }
    public void ClearSlot()
    {
        this.itemData = null;
        this.itemCount = -1;
    }
    public void AddToStack(int amount)
    {
        itemCount += amount;
    }
    public void RemoveFromStack(int amount)
    {
        itemCount -= amount;
    }
    public bool RoomLeftInStack(int amountToAdd,out int amountRemaining)
    {
        amountRemaining = itemData.MaxStack - itemCount;
        return RoomLeftInStack(amountToAdd);
    }
    public bool RoomLeftInStack(int amountToAdd)
    {
        if(itemCount + amountToAdd <= itemData.MaxStack)return true;
        else return false;
    }

    public void UpdateSlot(ItemData newItemData, int newItemCount)
    {
        this.itemData = newItemData;
        this.itemCount = newItemCount;
    }
}
