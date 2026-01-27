
using System;

[Serializable]
public class EquipmentInstance : ItemInstance
{
    public EquipmentData EquipmentData => itemData as EquipmentData;
    public float CurrentDurability;
    public bool IsEquipped = false;
    public string EquippedCharacterId;
    public EquipmentInstance(EquipmentData equipmentData) : base()
    {
        this.itemData = equipmentData;
        this.CurrentDurability = equipmentData.Durability;
    }
    
    public override object Clone()
    {
        return new EquipmentInstance(EquipmentData)
        {
            CurrentDurability = CurrentDurability,
            IsEquipped = IsEquipped,
            EquippedCharacterId = EquippedCharacterId
        };
    }
}