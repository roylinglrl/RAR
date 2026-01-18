using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySystem
{
    [SerializeField] private List<Item> inventoryItems = new List<Item>();
    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemRemoved;
    public event Action<Item> OnItemQuantityChanged;
    
    /// <summary>
    /// 添加物品到库存
    /// </summary>
    /// <param name="item">要添加的物品</param>
    /// <returns>是否成功添加物品</returns>
    public bool AddItem(Item item)
    {
        if (item.itemSO.itemStack > 1)
        {
            Item existingItem = inventoryItems.Find(x => x.itemID == item.itemID);
            if (existingItem != null)
            {
                existingItem.itemCount += item.itemCount;
                OnItemQuantityChanged?.Invoke(existingItem);
                return true;
            }
        }
        
        inventoryItems.Add(item);
        OnItemAdded?.Invoke(item);
        return true;
    }
    
    /// <summary>
    /// 从库存中移除物品
    /// </summary>
    /// <param name="item">要移除的物品</param>
    /// <returns>是否成功移除物品</returns>
    public bool RemoveItem(Item item)
    {
        if (inventoryItems.Remove(item))
        {
            OnItemRemoved?.Invoke(item);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 从库存中移除物品(指定数量)
    /// </summary>
    /// <param name="itemID">要移除的物品ID</param>
    /// <param name="quantity">要移除的物品数量</param>
    /// <returns>是否成功移除物品</returns>
    public bool RemoveItem(string itemID, int quantity = 1)
    {
        Item existingItem = inventoryItems.Find(x => x.itemID == itemID);
        if (existingItem != null)
        {
            if (existingItem.itemCount > quantity)
            {
                existingItem.itemCount -= quantity;
                OnItemQuantityChanged?.Invoke(existingItem);
                return true;
            }
            else
            {
                return RemoveItem(existingItem);
            }
        }
        return false;
    }
    
    /// <summary>
    /// 获取所有物品
    /// </summary>
    /// <returns>所有物品列表</returns>
    public List<Item> GetAllItems()
    {
        return inventoryItems;
    }
    
    /// <summary>
    /// 获取指定物品ID的物品
    /// </summary>
    /// <param name="itemID">要获取的物品ID</param>
    /// <returns>指定物品ID的物品</returns>
    public Item GetItem(string itemID)
    {
        return inventoryItems.Find(x => x.itemID == itemID);
    }
    
    public int GetItemQuantity(string itemID)
    {
        Item item = GetItem(itemID);
        return item != null ? item.itemCount : 0;
    }
    
    public bool HasItem(string itemID, int quantity = 1)
    {
        return GetItemQuantity(itemID) >= quantity;
    }
}
