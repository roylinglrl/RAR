using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using NUnit.Framework;

[Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;
    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => inventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>(size);
        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }
    public bool AddToInventory(ItemData itemData, int amount)
    {
    // 创建物品实例
        ItemInstance itemInstance = ItemFactory.CreateItemInstance(itemData);
        return AddToInventory(itemInstance, amount);
    }

    public bool AddToInventory(ItemInstance itemInstance, int amount)
    {
        if(ContainsItem(itemInstance.ItemData,out List<InventorySlot> slots))//如果物品已存在
        {
            foreach(InventorySlot slot in slots)//遍历所有槽位
            {
                if(slot.CanStackWith(new InventorySlot(itemInstance, amount)))//如果槽位有剩余空间
                {
                    if(slot.RoomLeftInStack(amount))//如果槽位有剩余空间
                    {
                        slot.AddToStack(amount);//添加到槽位
                        OnInventorySlotChanged?.Invoke(slot);//刷新槽位
                        return true;
                    }
                }
            }
        }
        while (amount > 0)
        {
            if(HasFreeSlot(out InventorySlot freeSlot))//如果有空闲槽位
            {
                int amountToAdd = Mathf.Min(amount, itemInstance.ItemData.MaxStack);//计算可添加数量
                ItemInstance slotInstance = (ItemInstance)itemInstance.Clone();
                freeSlot.UpdateSlot(itemInstance, amountToAdd);//更新槽位
                amount -= amountToAdd;//更新剩余数量
                OnInventorySlotChanged?.Invoke(freeSlot);//刷新槽位
            }
            else
            {
                return false;//没有空闲槽位
            }
        }
        return true;//添加成功
    }
    public bool ContainsItem(ItemData itemData,out List<InventorySlot> slots)
    {
        slots = inventorySlots.Where(slot => slot.ItemData != null && slot.ItemData == itemData).ToList();
        return slots.Count > 0 ? true : false;
    }
    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = inventorySlots.FirstOrDefault(slot => slot.ItemInstance == null);
        return freeSlot != null;
    }
    public void AddItemsFromLootTable(string lootTableID)
    {
        //TODO:从战利品表添加物品
    }
}