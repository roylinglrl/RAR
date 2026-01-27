using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class PlayerBackpackHolder : InventoryHolder
{
    [SerializeField] protected int _backpackSize = 10;
    [SerializeField] protected InventorySystem _backpackInventorySystem;
    private bool _isBackpackOpen = false;
    private float _currentBackpackCapacity = 0;
    public InventorySystem BackpackInventorySystem => _backpackInventorySystem;
    public static UnityAction<InventorySystem> OnPlayerBackpackDisplayRequested;
    protected override void Awake()
    {
        base.Awake();
        _backpackInventorySystem = new InventorySystem(_backpackSize);
    }
    void Update()
    {
        CheckBackpackCapacityChange();
        if(Keyboard.current.bKey.wasPressedThisFrame && GameManager.Instance.IsGameStart) 
        {
            ReadyBackpack();
            OnPlayerBackpackDisplayRequested?.Invoke(_backpackInventorySystem);
        }
    }
    public void ReadyBackpack()
    {
                    Debug.Log("打开背包");
            if(PlayerManager.Instance == null) return;
            if(PlayerManager.Instance.PlayerCombatEntity == null) return;
            if(PlayerManager.Instance.PlayerCombatEntity.attributeManager == null) return;
            if(!PlayerManager.Instance.PlayerCombatEntity.attributeManager.ContainsAttribute(AttributeType.BackPackCapacity)) return;
            float backPackCapacity = PlayerManager.Instance.PlayerCombatEntity.attributeManager.GetFinalAttributeValue(AttributeType.BackPackCapacity);
            Debug.Log("背包容量:" + backPackCapacity);
            int backpackSize = (int)backPackCapacity;
            _currentBackpackCapacity = backPackCapacity;
            if(_isBackpackOpen)
            {
                UpdateBackpackSize(backpackSize);
            }
            else
            {
                _backpackInventorySystem.Resize(backpackSize);
                _backpackSize = backpackSize;
            }
            _isBackpackOpen = true;
            GameManager.Instance.IsOnUI = true;
    }
        private void CheckBackpackCapacityChange()
    {
        if(PlayerManager.Instance == null) return;
        if(PlayerManager.Instance.PlayerCombatEntity == null) return;
        if(PlayerManager.Instance.PlayerCombatEntity.attributeManager == null) return;
        if(!PlayerManager.Instance.PlayerCombatEntity.attributeManager.ContainsAttribute(AttributeType.BackPackCapacity)) return;
        
        float newCapacity = PlayerManager.Instance.PlayerCombatEntity.attributeManager.GetFinalAttributeValue(AttributeType.BackPackCapacity);
        
        // 如果容量变化并且背包是打开的
        if(Mathf.Abs(newCapacity - _currentBackpackCapacity) > 0.01f && _isBackpackOpen)
        {
            Debug.Log("背包容量变化，从 " + _currentBackpackCapacity + " 变为 " + newCapacity);
            int newSize = (int)newCapacity;
            UpdateBackpackSize(newSize);
            _currentBackpackCapacity = newCapacity;
        }
    }
        private void UpdateBackpackSize(int newSize)
    {
        if(newSize == _backpackSize) return;
        
        if(newSize > _backpackSize)
        {
            // 增加容量，直接调整
            _backpackInventorySystem.Resize(newSize);
            _backpackSize = newSize;
            
            // 刷新UI
            if(_isBackpackOpen)
            {
                OnPlayerBackpackDisplayRequested?.Invoke(_backpackInventorySystem);
            }
        }
        else if(newSize < _backpackSize)
        {
            // 减少容量，需要处理可能溢出的物品
            HandleBackpackShrink(newSize);
        }
    }
        private void HandleBackpackShrink(int newSize)
    {
        // 获取将被移除的槽位中的物品
        List<InventorySlot> itemsToRelocate = new List<InventorySlot>();
        int slotsToRemove = _backpackSize - newSize;
        
        for(int i = 0; i < slotsToRemove; i++)
        {
            int index = _backpackSize - 1 - i;
            if(index >= 0 && index < _backpackInventorySystem.InventorySlots.Count)
            {
                var slot = _backpackInventorySystem.InventorySlots[index];
                if(slot.ItemInstance != null && slot.ItemCount > 0)
                {
                    // 复制物品到列表
                    itemsToRelocate.Add(new InventorySlot(
                        slot.ItemInstance.Clone() as ItemInstance,
                        slot.ItemCount
                    ));
                }
            }
        }
        
        // 先调整背包大小
        _backpackInventorySystem.Resize(newSize);
        _backpackSize = newSize;
        
        // 尝试重新安置物品
        RelocateOverflowItems(itemsToRelocate);
        
        // 刷新UI
        if(_isBackpackOpen)
        {
            OnPlayerBackpackDisplayRequested?.Invoke(_backpackInventorySystem);
        }
    }
        private void RelocateOverflowItems(List<InventorySlot> itemsToRelocate)
    {
        foreach(var itemSlot in itemsToRelocate)
        {
            if(itemSlot.ItemInstance == null) continue;
            
            // 先尝试放入背包剩余空间
            if(_backpackInventorySystem.AddToInventory(itemSlot.ItemInstance, itemSlot.ItemCount))
            {
                Debug.Log($"物品 {itemSlot.ItemInstance.ItemData.ItemName} 重新放入背包");
            }
            // 再尝试放入主背包
            else if(primaryInventorySystem.AddToInventory(itemSlot.ItemInstance, itemSlot.ItemCount))
            {
                Debug.Log($"物品 {itemSlot.ItemInstance.ItemData.ItemName} 放入主背包");
            }
            else
            {
                // 都放不下，掉落物品
                DropItem(itemSlot);
            }
        }
    }
        private void DropItem(InventorySlot itemSlot)
    {
        Debug.Log($"物品 {itemSlot.ItemInstance.ItemData.ItemName} x{itemSlot.ItemCount} 掉落");
        // TODO: 实现实际的掉落逻辑
        // 例如：生成掉落物预制体或调用掉落系统
    }
    public bool AddToInventory(ItemInstance itemInstance, int amount)
    {
        if(BackpackInventorySystem.AddToInventory(itemInstance, amount))
        {
            return true;
        }
        else if(primaryInventorySystem.AddToInventory(itemInstance, amount))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}