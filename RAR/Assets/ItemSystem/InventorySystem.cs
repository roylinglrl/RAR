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
        if(ContainsItem(itemData,out List<InventorySlot> slots))//如果物品已存在
        {
            foreach(InventorySlot slot in slots)//遍历所有槽位
            {
                if(slot.RoomLeftInStack(amount,out int amountRemaining))//如果槽位有剩余空间
                {
                    slot.AddToStack(amount);//添加到槽位
                    OnInventorySlotChanged?.Invoke(slot);//刷新槽位
                    return true;
                }
                else if(amountRemaining > 0)
                {
                    int canAdd = amountRemaining;
                    slot.AddToStack(canAdd);//添加到槽位
                    amount -= canAdd;
                    OnInventorySlotChanged?.Invoke(slot);//刷新槽位
                    return AddToInventory(itemData, amount);//递归添加剩余物品
                }
            }
        }
        else if(HasFreeSlot(out InventorySlot freeSlot))//如果有空闲槽位
        {
            freeSlot.UpdateSlot(itemData, amount);
            OnInventorySlotChanged?.Invoke(freeSlot);
            return true;
        }
        return false;
    }
    public bool ContainsItem(ItemData itemData,out List<InventorySlot> slots)
    {
        slots = inventorySlots.Where(slot => slot.ItemData == itemData).ToList();
        return slots.Count > 0 ? true : false;
    }
    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = inventorySlots.FirstOrDefault(slot => slot.ItemData == null);
        return freeSlot != null;
    }
}