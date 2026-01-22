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
        if(Keyboard.current.bKey.wasPressedThisFrame) 
        {
            Debug.Log("打开背包");
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