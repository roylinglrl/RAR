using System;

[Serializable]
public abstract class ItemInstance
{
    public ItemData ItemData{get;protected set;}
    public abstract object Clone();
}
[Serializable]
public class DurableItemInstance : ItemInstance
{
    public int CurrentDurability;
    public int MaxDurability;
    public DurableItemInstance(ItemData itemData,int maxDurability)
    {
        ItemData = itemData;
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