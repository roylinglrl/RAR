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
    public static UnityAction OnPlayerBackpackSizeChanged;
    private int _currentBackpackSize;
    protected override void Awake()
    {
        base.Awake();
        _backpackInventorySystem = new InventorySystem(_backpackSize);
        _backpackInventorySystem.OnPlayerBackpackSizeChanged += () => 
        {
            OnPlayerBackpackSizeChanged?.Invoke();
        };
    }
    void Update()
    {
        float initialCapacity = PlayerManager.Instance.PlayerCombatEntity.attributeManager.GetFinalAttributeValue(AttributeType.BackPackCapacity);
        _backpackSize = Mathf.Max(1, (int)initialCapacity);
        _currentBackpackSize = _backpackSize;
        int backpackSize = Mathf.Max(1, (int)initialCapacity);
        
        if (backpackSize != _currentBackpackSize)
        {
            // 背包大小发生变化，更新并刷新UI
            _backpackSize = backpackSize;
            _currentBackpackSize = backpackSize;
            
            // 调整背包大小
            _backpackInventorySystem.ResizeInventory(_backpackSize);
            
            Debug.Log($"背包大小已更新为: {_backpackSize}");
        }
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
    public void DropItem(ItemInstance itemInstance, int amount)
    {

    }
}