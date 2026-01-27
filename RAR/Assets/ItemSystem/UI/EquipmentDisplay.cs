
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDisplay : MonoBehaviour
{
    public CurrentItemData currentItemData;
    [Header("角色装备槽位")]
    [SerializeField] private EquipmentSlotUI armorSlotUI;
    [SerializeField] private EquipmentSlotUI energyModuleSlotUI;
    [SerializeField] private EquipmentSlotUI coreSlotUI;
    [SerializeField] private EquipmentSlotUI processorSlotUI;
    
    [Header("玩家装备槽位")]
    [SerializeField] private EquipmentSlotUI backpackSlotUI;
    [SerializeField] private EquipmentSlotUI[] weaponSlotsUI;
    
    [Header("当前角色信息")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterLevelText;

    private EquipmentManager equipmentManager;
    private CharacterManager characterManager;
    private SerializableDictionary<EquipmentSlotUI, EquipmentType> slotTypeMap = new SerializableDictionary<EquipmentSlotUI, EquipmentType>();
    private SerializableDictionary<EquipmentSlotUI, int> slotIndexMap = new SerializableDictionary<EquipmentSlotUI, int>();
    private void Awake()
    {
        equipmentManager = CharacterManager.Instance?.EquipmentManager;
        characterManager = CharacterManager.Instance;
        
        // 初始化槽位类型映射
        InitializeSlotTypeMap();
        
        // 初始化所有槽位的UI组件
        InitializeSlotUIs();
    }
     private void InitializeSlotTypeMap()
    {
        if (armorSlotUI != null) 
        {
            armorSlotUI.Initialize(EquipmentType.Armor);
            slotTypeMap[armorSlotUI] = EquipmentType.Armor;
            slotIndexMap[armorSlotUI] = 0;
        }
        if (energyModuleSlotUI != null) 
        {
            energyModuleSlotUI.Initialize(EquipmentType.EnergyModule);
            slotTypeMap[energyModuleSlotUI] = EquipmentType.EnergyModule;
            slotIndexMap[energyModuleSlotUI] = 0;
        }
        if (coreSlotUI != null) 
        {
            coreSlotUI.Initialize(EquipmentType.Core);
            slotTypeMap[coreSlotUI] = EquipmentType.Core;
            slotIndexMap[coreSlotUI] = 0;
        }
        if (processorSlotUI != null) 
        {
            processorSlotUI.Initialize(EquipmentType.Processor);
            slotTypeMap[processorSlotUI] = EquipmentType.Processor;
            slotIndexMap[processorSlotUI] = 0;
        }
        if (backpackSlotUI != null) 
        {
            backpackSlotUI.Initialize(EquipmentType.Backpack);
            slotTypeMap[backpackSlotUI] = EquipmentType.Backpack;
            slotIndexMap[backpackSlotUI] = 0;
        }
        
        for (int i = 0; i < weaponSlotsUI.Length && i < 2; i++)
        {
            if (weaponSlotsUI[i] != null)
            {
                weaponSlotsUI[i].Initialize(EquipmentType.Weapon, i);
                slotTypeMap[weaponSlotsUI[i]] = EquipmentType.Weapon;
                slotIndexMap[weaponSlotsUI[i]] = i;
            }
        }
    }
    
    private void InitializeSlotUIs()
    {
        // 订阅所有槽位的点击事件
        foreach (var slot in slotTypeMap.Keys)
        {
            if (slot != null)
            {
                slot.OnSlotClicked += OnEquipmentSlotClicked;
            }
        }
    }
    
    private void OnEnable()
    {
        if (equipmentManager != null)
        {
            equipmentManager.OnEquipmentChanged += OnEquipmentChanged;
            equipmentManager.OnPlayerEquipmentChanged += OnPlayerEquipmentChanged;
        }
        
        if (characterManager != null)
        {
            RefreshCharacterInfo();
        }
        
        RefreshAllEquipmentSlots();
    }
    
    private void OnDisable()
    {
        if (equipmentManager != null)
        {
            equipmentManager.OnEquipmentChanged -= OnEquipmentChanged;
            equipmentManager.OnPlayerEquipmentChanged -= OnPlayerEquipmentChanged;
        }
    }
    
    private void RefreshCharacterInfo()
    {
        if (characterManager == null || characterManager.currentCharacterData == null)
            return;
        
        var characterData = characterManager.currentCharacterData;
        
        if (characterNameText != null)
            characterNameText.text = characterData.CharacterName;
        
        if (characterLevelText != null)
            characterLevelText.text = $"Lv.{characterData.CurrentLevel}";
        
        if (characterIcon != null)
        {
            var characterSO = characterData.GetCharacterSO();
            if (characterSO != null)
                characterIcon.sprite = characterSO.CharacterIcon;
        }
    }
    
    private void RefreshAllEquipmentSlots()
    {
        RefreshCharacterEquipmentSlots();
        RefreshPlayerEquipmentSlots();
    }
    
    private void RefreshCharacterEquipmentSlots()
    {
        if (equipmentManager == null || characterManager?.currentCharacterData == null)
            return;
        
        var characterId = characterManager.currentCharacterData.CharacterID;
        
        armorSlotUI?.RefreshSlot(equipmentManager.GetEquippedItem(EquipmentType.Armor, characterId));
        energyModuleSlotUI?.RefreshSlot(equipmentManager.GetEquippedItem(EquipmentType.EnergyModule, characterId));
        coreSlotUI?.RefreshSlot(equipmentManager.GetEquippedItem(EquipmentType.Core, characterId));
        processorSlotUI?.RefreshSlot(equipmentManager.GetEquippedItem(EquipmentType.Processor, characterId));
    }

    
    private void RefreshPlayerEquipmentSlots()
    {
        if (equipmentManager == null)
            return;
        
        backpackSlotUI?.RefreshSlot(equipmentManager.GetEquippedItem(EquipmentType.Backpack));
        
        // 刷新武器槽位
        for (int i = 0; i < weaponSlotsUI.Length; i++)
        {
            if (weaponSlotsUI[i] != null)
            {
                // 获取对应索引的武器
                var weapon = equipmentManager.GetEquippedWeapon(i);
                weaponSlotsUI[i].RefreshSlot(weapon);
            }
        }
    }
    
private void OnEquipmentChanged(EquipmentType equipmentType, EquipmentInstance equipment)
{
    // 立即刷新对应的槽位
    switch (equipmentType)
    {
        case EquipmentType.Armor:
            armorSlotUI?.RefreshSlot(equipment);
            break;
        case EquipmentType.EnergyModule:
            energyModuleSlotUI?.RefreshSlot(equipment);
            break;
        case EquipmentType.Core:
            coreSlotUI?.RefreshSlot(equipment);
            break;
        case EquipmentType.Processor:
            processorSlotUI?.RefreshSlot(equipment);
            break;
    }
    
    UpdateCharacterAttributes();
}
    
    private void OnPlayerEquipmentChanged()
    {
        RefreshPlayerEquipmentSlots();
        UpdateCharacterAttributes();
    }
    
    private void UpdateCharacterAttributes()
    {
        characterManager?.UpdateAttributesFromEquipment();
    }
    
    public void OnEquipmentSlotClicked(EquipmentSlotUI slotUI)
    {
        if (slotUI == null || currentItemData == null)
            return;
        if (!slotTypeMap.TryGetValue(slotUI, out EquipmentType equipmentType) ||
            !slotIndexMap.TryGetValue(slotUI, out int slotIndex))
            return;    
        // 检查当前是否有物品被拿起
        EquipmentInstance slotEquipment = GetEquipmentFromSlot(slotUI, equipmentType, slotIndex);
        
        // 获取槽位类型
        var currentSlot = currentItemData.AssignedInventorySlot;
        bool hasCurrentItem = currentSlot.ItemInstance != null;
        bool hasSlotEquipment = slotEquipment != null;
        Debug.Log($"装备槽点击: CurrentItem为空={!hasCurrentItem}, 槽位有装备={hasSlotEquipment}");

            // 情况1: CurrentItem为空 且 槽位有装备 → 拿起装备
        if (!hasCurrentItem && hasSlotEquipment)
        {
            PickUpEquipment(slotUI, equipmentType, slotIndex, slotEquipment);
        }
        // 情况2: CurrentItem有装备 且 槽位为空 → 尝试装备
        else if (hasCurrentItem && !hasSlotEquipment)
        {
            TryEquipCurrentItem(slotUI, equipmentType, slotIndex, currentSlot);
        }
        // 情况3: CurrentItem有装备 且 槽位有装备 → 交换
        else if (hasCurrentItem && hasSlotEquipment)
        {
            SwapEquipment(slotUI, equipmentType, slotIndex, currentSlot, slotEquipment);
        }
    }
        private EquipmentInstance GetEquipmentFromSlot(EquipmentSlotUI slotUI, EquipmentType equipmentType, int slotIndex)
    {
        string characterId = null;
        
        if (IsCharacterEquipment(equipmentType))
        {
            characterId = characterManager?.currentCharacterData?.CharacterID;
            return equipmentManager?.GetEquippedItem(equipmentType, characterId);
        }
        else if (equipmentType == EquipmentType.Weapon)
        {
            return equipmentManager?.GetEquippedWeapon(slotIndex);
        }
        else
        {
            return equipmentManager?.GetEquippedItem(equipmentType);
        }
    }

    private void PickUpEquipment(EquipmentSlotUI slotUI, EquipmentType equipmentType, int slotIndex, EquipmentInstance slotEquipment)
    {
        // 从槽位取下装备，放到CurrentItemData中
        
        // 先创建一个InventorySlot来存放装备
        var inventorySlot = new InventorySlot(slotEquipment.Clone() as ItemInstance, 1);
        
        // 更新CurrentItemData
        currentItemData.UpdateItemSlot(inventorySlot, null); // 这里第二个参数是InventorySlotForUI，装备槽不需要
        
        // 卸下装备
        bool unequipSuccess = false;
        
        if (IsCharacterEquipment(equipmentType))
        {
            string characterId = characterManager?.currentCharacterData?.CharacterID;
            unequipSuccess = equipmentManager?.UnequipItem(equipmentType, characterId) ?? false;
        }
        else if (equipmentType == EquipmentType.Weapon)
        {
            unequipSuccess = equipmentManager?.UnequipWeapon(slotIndex) ?? false;
        }
        else
        {
            unequipSuccess = equipmentManager?.UnequipItem(equipmentType) ?? false;
        }
        
        // 关键：拿起装备后立即更新属性
        if (unequipSuccess && characterManager != null)
        {
            characterManager.UpdateAttributesFromEquipment();
        }
        
        Debug.Log($"拿起装备: {slotEquipment.ItemData.ItemName}, 卸下成功: {unequipSuccess}");
    }

    private void TryEquipCurrentItem(EquipmentSlotUI slotUI, EquipmentType equipmentType, int slotIndex, InventorySlot currentSlot)
    {
        // 检查当前物品是否是装备
        if (currentSlot.ItemInstance is EquipmentInstance equipmentToEquip)
        {
            // 检查装备类型是否匹配
            if (equipmentToEquip.EquipmentData.EquipmentType != equipmentType)
            {
                Debug.Log($"装备类型不匹配！需要{equipmentType}，当前是{equipmentToEquip.EquipmentData.EquipmentType}");
                return;
            }
            
            // 检查装备要求等级
            if (characterManager?.currentCharacterData != null)
            {
                if (equipmentToEquip.EquipmentData.RequiredLevel > characterManager.currentCharacterData.CurrentLevel)
                {
                    Debug.Log($"装备要求等级不足！需要{equipmentToEquip.EquipmentData.RequiredLevel}级");
                    return;
                }
            }
            
            // 装备物品
            bool success = false;
            
            if (IsCharacterEquipment(equipmentType))
            {
                string characterId = characterManager?.currentCharacterData?.CharacterID;
                success = equipmentManager?.EquipItem(equipmentToEquip, characterId) ?? false;
            }
            else if (equipmentType == EquipmentType.Weapon)
            {
                success = equipmentManager?.EquipWeapon(equipmentToEquip, slotIndex) ?? false;
            }
            else
            {
                success = equipmentManager?.EquipItem(equipmentToEquip) ?? false;
            }
            
            if (success)
            {
                // 清空CurrentItemData
                currentItemData.CloseSlot();
                if (characterManager != null)
                {
                    characterManager.UpdateAttributesFromEquipment();
                }
                Debug.Log($"装备{equipmentToEquip.ItemData.ItemName}成功！");
            }
            else
            {
                Debug.Log($"装备{equipmentToEquip.ItemData.ItemName}失败！");
            }
        }
        else
        {
            Debug.Log("当前物品不是装备，无法装备！");
        }
    }
    
    private void SwapEquipment(EquipmentSlotUI slotUI, EquipmentType equipmentType, int slotIndex, 
                              InventorySlot currentSlot, EquipmentInstance slotEquipment)
    {
        // 检查当前物品是否是装备
        if (!(currentSlot.ItemInstance is EquipmentInstance equipmentToEquip))
        {
            Debug.Log("当前物品不是装备！");
            return;
        }
        
        // 检查装备类型是否匹配
        if (equipmentToEquip.EquipmentData.EquipmentType != equipmentType)
        {
            Debug.Log($"装备类型不匹配！需要{equipmentType}，当前是{equipmentToEquip.EquipmentData.EquipmentType}");
            return;
        }
        
        // 检查装备要求等级
        if (characterManager?.currentCharacterData != null)
        {
            if (equipmentToEquip.EquipmentData.RequiredLevel > characterManager.currentCharacterData.CurrentLevel)
            {
                Debug.Log($"装备要求等级不足！需要{equipmentToEquip.EquipmentData.RequiredLevel}级");
                return;
            }
        }
        
        // 先卸下当前槽位的装备
        EquipmentInstance unequippedEquipment = null;
        
        if (IsCharacterEquipment(equipmentType))
        {
            string characterId = characterManager?.currentCharacterData?.CharacterID;
            if (equipmentManager?.UnequipItem(equipmentType, characterId) ?? false)
            {
                unequippedEquipment = slotEquipment;
            }
        }
        else if (equipmentType == EquipmentType.Weapon)
        {
            if (equipmentManager?.UnequipWeapon(slotIndex) ?? false)
            {
                unequippedEquipment = slotEquipment;
            }
        }
        else
        {
            if (equipmentManager?.UnequipItem(equipmentType) ?? false)
            {
                unequippedEquipment = slotEquipment;
            }
        }
        
        if (unequippedEquipment == null)
        {
            Debug.Log("卸下装备失败！");
            return;
        }
        
        // 装备新物品
        bool equipSuccess = false;
        
        if (IsCharacterEquipment(equipmentType))
        {
            string characterId = characterManager?.currentCharacterData?.CharacterID;
            equipSuccess = equipmentManager?.EquipItem(equipmentToEquip, characterId) ?? false;
        }
        else if (equipmentType == EquipmentType.Weapon)
        {
            equipSuccess = equipmentManager?.EquipWeapon(equipmentToEquip, slotIndex) ?? false;
        }
        else
        {
            equipSuccess = equipmentManager?.EquipItem(equipmentToEquip) ?? false;
        }
        
        if (equipSuccess)
        {
            // 将卸下的装备放到CurrentItemData中
            var inventorySlot = new InventorySlot(unequippedEquipment.Clone() as ItemInstance, 1);
            currentItemData.UpdateItemSlot(inventorySlot, null);
            if (characterManager != null)
            {
                characterManager.UpdateAttributesFromEquipment();
            }            
            Debug.Log($"交换装备成功！装备了{equipmentToEquip.ItemData.ItemName}，拿起了{unequippedEquipment.ItemData.ItemName}");
        }
        else
        {
            // 如果装备失败，把卸下的装备装回去
            Debug.Log("装备新物品失败，恢复原装备");
            // 这里需要重新装备原装备
        }
    }
    
    private bool IsCharacterEquipment(EquipmentType equipmentType)
    {
        return equipmentType == EquipmentType.Armor ||
               equipmentType == EquipmentType.EnergyModule ||
               equipmentType == EquipmentType.Core ||
               equipmentType == EquipmentType.Processor;
    }
}
