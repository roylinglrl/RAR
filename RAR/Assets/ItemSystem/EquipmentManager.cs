using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentManager
{
    [SerializeField] private SerializableDictionary<string, EquipmentSlot> characterEquipmentSlots = new SerializableDictionary<string, EquipmentSlot>();
    [SerializeField] private SerializableDictionary<EquipmentType, EquipmentSlot> playerEquipmentSlots = new SerializableDictionary<EquipmentType, EquipmentSlot>();
    [SerializeField] private SerializableDictionary<int, EquipmentSlot> weaponSlots = new SerializableDictionary<int, EquipmentSlot>(); // 武器槽索引
    
    public CharacterData CurrentCharacter { get; private set; }
    public event Action<EquipmentType, EquipmentInstance> OnEquipmentChanged;
    public event Action OnPlayerEquipmentChanged;
        public EquipmentManager()
    {
        InitializePlayerEquipmentSlots();
    }
        private void InitializePlayerEquipmentSlots()
    {
        // 玩家绑定的装备槽
        playerEquipmentSlots[EquipmentType.Backpack] = new EquipmentSlot(EquipmentType.Backpack);
        playerEquipmentSlots[EquipmentType.Weapon] = new EquipmentSlot(EquipmentType.Weapon, 2); // 两个武器槽
                for (int i = 0; i < 2; i++)
        {
            weaponSlots[i] = new EquipmentSlot();
            weaponSlots[i].AddSlot(EquipmentType.Weapon);
        }
        // 初始化每个已解锁角色的装备槽
        if (CharacterManager.Instance != null)
        {
            foreach (var character in CharacterManager.Instance.UnlockedCharacters)
            {
                InitializeCharacterEquipmentSlots(character.CharacterID);
            }
            
            if (CharacterManager.Instance.currentCharacterData != null)
            {
                SetCurrentCharacter(CharacterManager.Instance.currentCharacterData.CharacterID);
            }
        }
    }
    
    private void InitializeCharacterEquipmentSlots(string characterId)
    {
        if (!characterEquipmentSlots.ContainsKey(characterId))
        {
            characterEquipmentSlots[characterId] = new EquipmentSlot();
            
            // 添加角色绑定的装备槽
            characterEquipmentSlots[characterId].AddSlot(EquipmentType.Armor);
            characterEquipmentSlots[characterId].AddSlot(EquipmentType.EnergyModule);
            characterEquipmentSlots[characterId].AddSlot(EquipmentType.Core);
            characterEquipmentSlots[characterId].AddSlot(EquipmentType.Processor);
        }
    }
    
    public void SetCurrentCharacter(string characterId)
    {
        InitializeCharacterEquipmentSlots(characterId);
        
        if (CharacterManager.Instance != null)
        {
            CurrentCharacter = CharacterManager.Instance.GetCharacterData(characterId);
            if (CurrentCharacter == null)
            {
                Debug.LogError($"Character not found: {characterId}");
            }
        }
    }
    
    public bool EquipItem(EquipmentInstance equipment, string characterId = null)
    {
        if (equipment == null || equipment.EquipmentData == null)
            return false;
        
        var equipmentType = equipment.EquipmentData.EquipmentType;
        
        // 判断是角色装备还是玩家装备
        if (IsCharacterEquipment(equipmentType))
        {
            if (string.IsNullOrEmpty(characterId))
                characterId = CurrentCharacter?.CharacterID;
            
            if (string.IsNullOrEmpty(characterId))
                return false;
            
            return EquipToCharacter(characterId, equipment);
        }
        else
        {
            return EquipToPlayer(equipment);
        }
    }
    
    private bool EquipToCharacter(string characterId, EquipmentInstance equipment)
    {
        if (!characterEquipmentSlots.ContainsKey(characterId))
            return false;
        
        var slot = characterEquipmentSlots[characterId];
        if (slot.EquipItem(equipment))
        {
            equipment.IsEquipped = true;
            equipment.EquippedCharacterId = characterId;
            OnEquipmentChanged?.Invoke(equipment.EquipmentData.EquipmentType, equipment);
            return true;
        }
        
        return false;
    }
    
    private bool EquipToPlayer(EquipmentInstance equipment)
    {
        var equipmentType = equipment.EquipmentData.EquipmentType;
        
        if (equipmentType == EquipmentType.Weapon)
        {
            // 武器槽可能有多个，找到第一个空闲的
            var weaponSlot = playerEquipmentSlots[EquipmentType.Weapon];
            if (weaponSlot != null && weaponSlot.CanEquip(equipment))
            {
                // 这里简化处理，实际可能需要找到具体的武器槽位
                equipment.IsEquipped = true;
                OnPlayerEquipmentChanged?.Invoke();
                return true;
            }
        }
        else if (playerEquipmentSlots.ContainsKey(equipmentType))
        {
            var slot = playerEquipmentSlots[equipmentType];
            if (slot.EquipItem(equipment))
            {
                equipment.IsEquipped = true;
                OnPlayerEquipmentChanged?.Invoke();
                return true;
            }
        }
        
        return false;
    }
    
public bool UnequipItem(EquipmentType equipmentType, string characterId = null)
{
    if (IsCharacterEquipment(equipmentType))
    {
        if (string.IsNullOrEmpty(characterId))
            characterId = CurrentCharacter?.CharacterID;
        
        if (string.IsNullOrEmpty(characterId) || !characterEquipmentSlots.ContainsKey(characterId))
            return false;
        
        var slot = characterEquipmentSlots[characterId];
        if (slot.UnequipItem(equipmentType))
        {
            // 关键：触发装备变更事件
            OnEquipmentChanged?.Invoke(equipmentType, null);
            return true;
        }
    }
    else
    {
        if (playerEquipmentSlots.ContainsKey(equipmentType))
        {
            if (playerEquipmentSlots[equipmentType].UnequipItem(equipmentType))
            {
                OnPlayerEquipmentChanged?.Invoke();
                return true;
            }
        }
    }
    
    return false;
}
    
    public EquipmentInstance GetEquippedItem(EquipmentType equipmentType, string characterId = null)
    {
        if (IsCharacterEquipment(equipmentType))
        {
            if (string.IsNullOrEmpty(characterId))
                characterId = CurrentCharacter?.CharacterID;
            
            if (string.IsNullOrEmpty(characterId) || !characterEquipmentSlots.ContainsKey(characterId))
                return null;
            
            return characterEquipmentSlots[characterId].GetEquippedItem(equipmentType);
        }
        else
        {
            if (playerEquipmentSlots.ContainsKey(equipmentType))
            {
                return playerEquipmentSlots[equipmentType].GetEquippedItem(equipmentType);
            }
        }
        
        return null;
    }
    
    private bool IsCharacterEquipment(EquipmentType equipmentType)
    {
        return equipmentType == EquipmentType.Armor ||
               equipmentType == EquipmentType.EnergyModule ||
               equipmentType == EquipmentType.Core ||
               equipmentType == EquipmentType.Processor;
    }
    
    public List<AttributeModifier> GetAllEquipmentModifiers(string characterId = null)
    {
        var modifiers = new List<AttributeModifier>();
        
        if (string.IsNullOrEmpty(characterId))
            characterId = CurrentCharacter?.CharacterID;
        
        // 角色装备加成
        if (!string.IsNullOrEmpty(characterId) && characterEquipmentSlots.ContainsKey(characterId))
        {
            var characterModifiers = characterEquipmentSlots[characterId].GetAllModifiers();
            modifiers.AddRange(characterModifiers);
        }
        
        // 玩家装备加成（背包和武器）
        foreach (var slot in playerEquipmentSlots.Values)
        {
            var slotModifiers = slot.GetAllModifiers();
            modifiers.AddRange(slotModifiers);
        }
        
        return modifiers;
    }
        public EquipmentInstance GetEquippedWeapon(int slotIndex)
    {
        if (weaponSlots.TryGetValue(slotIndex, out EquipmentSlot slot))
        {
            return slot.GetEquippedItem(EquipmentType.Weapon);
        }
        return null;
    }
    public bool EquipWeapon(EquipmentInstance weapon, int slotIndex)
    {
        if (weapon == null || weapon.EquipmentData == null || 
            weapon.EquipmentData.EquipmentType != EquipmentType.Weapon)
            return false;
        
        if (weaponSlots.TryGetValue(slotIndex, out EquipmentSlot slot))
        {
            if (slot.CanEquip(weapon))
            {
                var currentWeapon = slot.GetEquippedItem(EquipmentType.Weapon);
                if (currentWeapon != null)
                {
                    currentWeapon.IsEquipped = false;
                }
                
                slot.EquipItem(weapon);
                weapon.IsEquipped = true;
                OnPlayerEquipmentChanged?.Invoke();
                return true;
            }
        }
        
        return false;
    }
    
    public bool UnequipWeapon(int slotIndex)
    {
        if (weaponSlots.TryGetValue(slotIndex, out EquipmentSlot slot))
        {
            var weapon = slot.GetEquippedItem(EquipmentType.Weapon);
            if (weapon != null)
            {
                weapon.IsEquipped = false;
                slot.UnequipItem(EquipmentType.Weapon);
                OnPlayerEquipmentChanged?.Invoke();
                return true;
            }
        }
        
        return false;
    }
    
}