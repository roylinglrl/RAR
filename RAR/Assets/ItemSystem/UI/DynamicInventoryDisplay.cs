
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField]protected InventorySlotForUI SlotPrefab;
    private InventoryHolder currentInventoryHolder;
    protected override void Start()
    {
        //InventoryHolder.OnDynamicInventoryDisplayRequested += RefreshDynamicInventoryDisplay;
        base.Start();
        //AssignSlot(inventorySystem);
    }
    private void OnDestroy()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= RefreshDynamicInventoryDisplay;
    }
    //刷新动态物品显示
    public void RefreshDynamicInventoryDisplay(InventorySystem inventoryToDisplay,InventoryHolder holder=null)
    {
        ClearSlots();
        inventorySystem = inventoryToDisplay;
        currentInventoryHolder = holder;
        if(inventorySystem != null) 
        {
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
            inventorySystem.OnInventorySizeChanged += HandleInventorySizeChanged;
        }
        AssignSlot(inventoryToDisplay);

    }
        private void HandleInventorySizeChanged()
    {
        AssignSlot(inventorySystem);
    }
    public override void AssignSlot(InventorySystem inventoryToDisplay)
    {
        ClearSlots();
        slotForUI = new SerializableDictionary<InventorySlotForUI, InventorySlot>();
        if(inventoryToDisplay == null) return;
        for(int i = 0; i < inventoryToDisplay.InventorySize; i++)
        {
            var slot = Instantiate(SlotPrefab, transform);
            slotForUI.Add(slot, inventoryToDisplay.InventorySlots[i]);
            slot.Init(inventoryToDisplay.InventorySlots[i]);
            slot.UpdateSlotUI();
        }
    }
    private void ClearSlots()
    {
        foreach(var item in transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }
        if(slotForUI != null)
        {
            slotForUI.Clear();
        }
    }
    private void OnDisable()
    {
        if(inventorySystem != null) 
        {
            inventorySystem.OnInventorySlotChanged -= UpdateSlot;
            inventorySystem.OnInventorySizeChanged -= HandleInventorySizeChanged;
        }
    }
}