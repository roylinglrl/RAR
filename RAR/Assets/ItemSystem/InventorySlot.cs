using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData itemData;//物品数据,后续将会替换为物品实例以储存可变变量
    [SerializeField] private int itemCount;//物品数量
    public ItemData ItemData => itemData;
    [SerializeField]public int ItemCount => itemCount;
    //物品槽的构造函数
    public InventorySlot(ItemData sourceItemData, int amount)
    {
        this.itemData = sourceItemData;
        this.itemCount = amount;
    }
    //物品槽的默认构造函数
    public InventorySlot()
    {
        ClearSlot();
    }
    //将新物品分配给物品槽
    public void AssignItem(InventorySlot newInventorySlot)
    {
        if(itemData == newInventorySlot.ItemData)
        AddToStack(newInventorySlot.ItemCount);
        else
        {
            itemData = newInventorySlot.ItemData;
            itemCount = 0;
            AddToStack(newInventorySlot.ItemCount);
        }
    }
    //清空物品槽
    public void ClearSlot()
    {
        this.itemData = null;
        this.itemCount = -1;
    }
    //将物品添加到物品槽的栈中
    public void AddToStack(int amount)
    {
        itemCount += amount;
    }
    //从物品槽的栈中移除物品
    public void RemoveFromStack(int amount)
    {
        itemCount -= amount;
    }
    //检查物品槽是否有足够的空间来添加物品
    public bool RoomLeftInStack(int amountToAdd,out int amountRemaining)
    {
        amountRemaining = itemData != null ? itemData.MaxStack - itemCount : 0;
        return amountRemaining >= amountToAdd;
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
    public bool SplitStack(out InventorySlot splitSlot)
    {
        if(itemCount <= 1)
        {
            splitSlot = null;
            return false;
        }
        int half = Mathf.RoundToInt(itemCount / 2);
        RemoveFromStack(half);
        splitSlot = new InventorySlot(itemData, half);
        return true;
    }
}
