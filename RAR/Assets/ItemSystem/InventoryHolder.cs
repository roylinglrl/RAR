    using System;
    using UnityEngine;
    using UnityEngine.Events;
    
    [Serializable]
    public class InventoryHolder : MonoBehaviour
    {
        [SerializeField] private int InventorySize;
        [SerializeField] protected InventorySystem primaryInventorySystem;
        public InventorySystem PrimaryInventorySystem => primaryInventorySystem; // 修改属性名为PascalCase
        public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;
        
        protected virtual void Awake()
        {
            primaryInventorySystem = new InventorySystem(InventorySize);
        }
    }