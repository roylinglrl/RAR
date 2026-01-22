using UnityEngine;

public static class ItemFactory
{
    public static ItemInstance CreateItemInstance(ItemData itemData)
    {
        return new StaticItemInstance(itemData);
    }
}