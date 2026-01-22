using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class ItemInstance
{
    [SerializeReference] protected ItemData itemData;
    public ItemData ItemData => itemData;
    public abstract object Clone();
}
[Serializable]
public class StaticItemInstance : ItemInstance
{
    public StaticItemInstance(ItemData newItemData)
    {
        this.itemData = newItemData;
    }
    public override object Clone()
    {
        return new StaticItemInstance(ItemData);
    }
}
[Serializable]
public class DurableItemInstance : ItemInstance
{
    public int CurrentDurability;
    public int MaxDurability;
    public DurableItemInstance(ItemData itemData,int maxDurability)
    {
        this.itemData = itemData;
        MaxDurability = maxDurability;
        CurrentDurability = maxDurability;
    }
    public override object Clone()
    {
        return new DurableItemInstance(ItemData, MaxDurability)
        {
            CurrentDurability = CurrentDurability,
        };
    }
}