using UnityEngine;

public static class ItemFactory
{
    public static ItemInstance CreateItemInstance(ItemData itemData)
    {
        if (itemData is EquipmentData equipmentData)
        {
            return new EquipmentInstance(equipmentData);
        }
        else
        {
            return new StaticItemInstance(itemData);
        }
    }
}