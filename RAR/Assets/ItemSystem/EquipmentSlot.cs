using System.Collections.Generic;
using System;
using UnityEngine;
[Serializable]
public class EquipmentSlot
{
    [SerializeField] private SerializableDictionary<EquipmentType, EquipmentInstance> equippedItems = new SerializableDictionary<EquipmentType, EquipmentInstance>();
    [SerializeField] private SerializableDictionary<EquipmentType, List<EquipmentInstance>> multiSlotItems = new SerializableDictionary<EquipmentType, List<EquipmentInstance>>();
    
    public EquipmentSlot()
    {
    }
    
    public EquipmentSlot(EquipmentType type, int slotCount = 1)
    {
        if (slotCount == 1)
        {
            AddSlot(type);
        }
        else
        {
            for (int i = 0; i < slotCount; i++)
            {
                AddSlot(type);
            }
        }
    }
    
    public void AddSlot(EquipmentType type)
    {
        if (!equippedItems.ContainsKey(type))
        {
            equippedItems[type] = null;
        }
    }
    
    public bool EquipItem(EquipmentInstance equipment)
    {
        var equipmentType = equipment.EquipmentData.EquipmentType;
        
        if (equippedItems.ContainsKey(equipmentType))
        {
            // 单槽位装备
            if (equippedItems[equipmentType] == null)
            {
                equippedItems[equipmentType] = equipment;
                return true;
            }
        }
        
        return false;
    }
    
    public bool UnequipItem(EquipmentType equipmentType)
    {
        if (equippedItems.ContainsKey(equipmentType))
        {
            if (equippedItems[equipmentType] != null)
            {
                equippedItems[equipmentType].IsEquipped = false;
                equippedItems[equipmentType].EquippedCharacterId = null;
                equippedItems[equipmentType] = null;
                return true;
            }
        }
        
        return false;
    }
    
    public EquipmentInstance GetEquippedItem(EquipmentType equipmentType)
    {
        if (equippedItems.ContainsKey(equipmentType))
        {
            return equippedItems[equipmentType];
        }
        
        return null;
    }
    
    public bool CanEquip(EquipmentInstance equipment)
    {
        var equipmentType = equipment.EquipmentData.EquipmentType;
        
        if (equippedItems.ContainsKey(equipmentType))
        {
            return equippedItems[equipmentType] == null;
        }
        
        return false;
    }
    
    public List<AttributeModifier> GetAllModifiers()
    {
        var modifiers = new List<AttributeModifier>();
        
        foreach (var item in equippedItems.Values)
        {
            if (item != null && item.EquipmentData != null && item.EquipmentData.AttributeModifiersList != null)
            {
                modifiers.AddRange(item.EquipmentData.AttributeModifiersList);
            }
        }
        
        return modifiers;
    }
    public List<EquipmentInstance> GetAllEquippedItems()
    {
        var items = new List<EquipmentInstance>();
        
        foreach (var item in equippedItems.Values)
        {
            if (item != null && item is EquipmentInstance equipment)
            {
                items.Add(equipment);
            }
        }
        
        return items;
    }
}
