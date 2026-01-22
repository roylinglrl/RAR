using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    [SerializeField] private ItemInstance itemInstance;//物品数据,后续将会替换为物品实例以储存可变变量
    [SerializeField] private int itemCount;//物品数量
    public ItemInstance ItemData => itemInstance;
    [SerializeField]public int ItemCount => itemCount;
    //物品槽的构造函数
    public InventorySlot(ItemInstance sourceItemData, int amount)
    {
        this.itemInstance = sourceItemData;
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
        if(newInventorySlot.itemInstance != null)
        {
            this.itemInstance = newInventorySlot.itemInstance.Clone() as ItemInstance;
            this.itemCount = newInventorySlot.ItemCount;
        }
        else
        {
            ClearSlot();
        }
    }
    public bool CanStackWith(InventorySlot otherSlot)
    {
        if(itemInstance == null || otherSlot.itemInstance == null)return false;
        if(itemInstance.ItemData != otherSlot.itemInstance.ItemData)return false;
        if(itemInstance.ItemData.MaxStack == 1)return false;
        return true;
    }
    //清空物品槽
    public void ClearSlot()
    {
        this.itemInstance = null;
        this.itemCount = 0;
    }
    //将物品添加到物品槽的栈中
    public void AddToStack(int amount)
    {
        itemCount += amount;
    }
    //从物品槽的栈中移除物品
    public void RemoveFromStack(int amount)
    {
        itemCount = Mathf.Max(0, itemCount - amount);
        if (itemCount == 0)
        {
            ClearSlot();
        }
    }
    //检查物品槽是否有足够的空间来添加物品
    public bool RoomLeftInStack(int amountToAdd,out int amountRemaining)
    {
        amountRemaining = itemInstance != null ? itemInstance.ItemData.MaxStack - itemCount : 0;
        return amountRemaining >= amountToAdd;
    }
    public bool RoomLeftInStack(int amountToAdd)
    {
        if(itemCount + amountToAdd <= itemInstance.ItemData.MaxStack)return true;
        else return false;
    }

    public void UpdateSlot(ItemInstance  newItemInstance, int newItemCount)
    {
        this.itemInstance = newItemInstance;
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
        var cloneInstance = itemInstance.Clone() as ItemInstance;
        splitSlot = new InventorySlot(cloneInstance, half);
        return true;
    }
}
