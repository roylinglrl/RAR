using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    [SerializeReference] private ItemInstance itemInstance;//物品数据,后续将会替换为物品实例以储存可变变量
    [SerializeField] private int itemCount;//物品数量
    public ItemData ItemData => itemInstance?.ItemData;
    public ItemInstance ItemInstance => itemInstance;
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
        if(newInventorySlot.ItemInstance != null)
        {
            this.itemInstance = newInventorySlot.ItemInstance.Clone() as ItemInstance;
            this.itemCount = newInventorySlot.ItemCount;
        }
        else
        {
            ClearSlot();
        }
    }
    public bool CanStackWith(InventorySlot otherSlot)
    {
        if(ItemInstance == null || otherSlot.ItemInstance == null)return false;
        if(ItemInstance.ItemData != otherSlot.ItemInstance.ItemData)return false;
        if(ItemInstance.ItemData.MaxStack == 1)return false;
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
        amountRemaining = ItemInstance != null ? ItemInstance.ItemData.MaxStack - itemCount : 0;
        return amountRemaining >= amountToAdd;
    }
    public bool RoomLeftInStack(int amountToAdd)
    {
        if(itemCount + amountToAdd <= ItemInstance.ItemData.MaxStack)return true;
        else return false;
    }

    public void UpdateSlot(ItemInstance  newItemInstance, int newItemCount)
    {
        itemInstance = newItemInstance;
        itemCount = newItemCount;
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
        var cloneInstance = ItemInstance.Clone() as ItemInstance;
        splitSlot = new InventorySlot(cloneInstance, half);
        return true;
    }
}
