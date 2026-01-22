using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerBackpackHolder : InventoryHolder
{
    [SerializeField] protected int _backpackSize = 10;
    [SerializeField] protected InventorySystem _backpackInventorySystem;
    public InventorySystem BackpackInventorySystem => _backpackInventorySystem;

    protected override void Awake()
    {
        base.Awake();
        _backpackInventorySystem = new InventorySystem(_backpackSize);
    }
    void Update()
    {
        if(Keyboard.current.bKey.wasPressedThisFrame) 
        {
            OnDynamicInventoryDisplayRequested?.Invoke(_backpackInventorySystem);
        }
    }
    public bool AddToInventory(ItemData data, int amount)
    {
        if(primaryInventorySystem.AddToInventory(data, amount))
        {
            return true;
        }
        else if(BackpackInventorySystem.AddToInventory(data, amount))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}