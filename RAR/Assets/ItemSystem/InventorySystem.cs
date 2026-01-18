using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySystem
{
    public int BackpackCount { get; private set; }//背包容量
    [SerializeField] private List<Item> inventoryItems = new List<Item>();// 库存物品列表
    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemRemoved;
    public event Action<Item> OnItemQuantityChanged;

    public void init()//初始化库存系统
    {
        refreshBackpackCount();
    }
    public bool canChangeBackpackCount(int newBackpackCount)
    {
        return newBackpackCount >= BackpackCount;
    }//判断是否可以改变背包容量
    public void refreshBackpackCount()
    {
        if(CharacterManager.Instance.currentCharacterData == null)//如果当前角色数据为空
        {
            return;
        }
        CharacterManager.Instance.attributeManager._isDirty = true;//标记属性系统为脏
        int BackpackCount = (int)CharacterManager.Instance.attributeManager.GetFinalAttributeValue(AttributeType.BackPackCapacity);
        if(BackpackCount >= 0 && BackpackCount != this.BackpackCount)
        {
            this.BackpackCount = BackpackCount;
        }
        List<Item> oldItems = new List<Item>();
        foreach (Item item in inventoryItems)
        {
            if(item != null)
            {
                oldItems.Add(item);
            }
        }
        inventoryItems.Clear();
        inventoryItems.AddRange(oldItems);
    }

    public bool AddItem(Item item)//添加物品到库存的第一个null位置
    {
        //寻找第一个null的位置
        int firstNullIndex = inventoryItems.IndexOf(null);
        if (firstNullIndex != -1)
        {
            inventoryItems[firstNullIndex] = item;
            OnItemAdded?.Invoke(item);
            return true;
        }
        return false;
    }
    public bool RemoveItem(Item item)//从库存中移除物品
    {
        int index = inventoryItems.IndexOf(item);
        if (index != -1)
        {
            inventoryItems[index] = null;
            OnItemRemoved?.Invoke(item);
            return true;
        }
        return false;
    }
    public void RemoveItemWithPosition(int position)//从库存中移除物品(指定位置)
    {
        if (position >= 0 && position < inventoryItems.Count)
        {
            Item item = inventoryItems[position];
            if (item != null)
            {
                inventoryItems[position] = null;
                OnItemRemoved?.Invoke(item);
            }
        }
    }
    public void AddItemWithPosition(Item item, int position)//在库存中添加物品(指定位置)
    {
        if (position >= 0 && position < inventoryItems.Count)
        {
            if(inventoryItems[position] == null){
                inventoryItems[position] = item;
                OnItemAdded?.Invoke(item);
            }
            else
            {
                if(inventoryItems[position].itemID == item.itemID)
                {
                    inventoryItems[position].itemCount += item.itemCount;
                    OnItemQuantityChanged?.Invoke(item);
                }
                else
                {
                    Debug.LogError("尝试在已有的物品位置添加不同的物品");
                }
            }
        }
    }
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
