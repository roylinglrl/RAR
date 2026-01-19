using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InventoryHolder:MonoBehaviour
{
    [SerializeField] private int InventorySize;
    [SerializeField] protected InventorySystem inventorySystem;
    public InventorySystem InventorySystem => inventorySystem;
    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;
    
    private void Awake()
    {
        inventorySystem = new InventorySystem(InventorySize);
    }
}