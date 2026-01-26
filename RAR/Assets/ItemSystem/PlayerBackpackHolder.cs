using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class PlayerBackpackHolder : InventoryHolder
{
    [SerializeField] protected int _backpackSize = 10;
    [SerializeField] protected InventorySystem _backpackInventorySystem;
    public InventorySystem BackpackInventorySystem => _backpackInventorySystem;
    public static UnityAction<InventorySystem> OnPlayerBackpackDisplayRequested;
    protected override void Awake()
    {
        base.Awake();
        _backpackInventorySystem = new InventorySystem(_backpackSize);
    }
    void Update()
    {
        if(Keyboard.current.bKey.wasPressedThisFrame && GameManager.Instance.IsGameStart) 
        {
            Debug.Log("打开背包");
            if(PlayerManager.Instance == null) return;
            if(PlayerManager.Instance.PlayerCombatEntity == null) return;
            if(PlayerManager.Instance.PlayerCombatEntity.attributeManager == null) return;
            if(!PlayerManager.Instance.PlayerCombatEntity.attributeManager.ContainsAttribute(AttributeType.BackPackCapacity)) return;
            float backPackCapacity = PlayerManager.Instance.PlayerCombatEntity.attributeManager.GetFinalAttributeValue(AttributeType.BackPackCapacity);
            Debug.Log("背包容量:" + backPackCapacity);
            int backpackSize = (int)backPackCapacity;
            _backpackInventorySystem.Resize(backpackSize);
            _backpackSize = backpackSize;
            OnPlayerBackpackDisplayRequested?.Invoke(_backpackInventorySystem);
        }
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