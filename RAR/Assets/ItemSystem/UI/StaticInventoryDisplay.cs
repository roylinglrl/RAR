using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField]protected InventoryHolder inventoryHolder;
    [SerializeField]protected InventorySlotForUI[] inventorySlotForUI;
    protected override void Start()
    {
        base.Start();
        if(inventoryHolder != null)//判断背包持有者不为空
        {
            inventorySystem = inventoryHolder.PrimaryInventorySystem;//获取物品槽
            AssignSlot(inventoryHolder.PrimaryInventorySystem);
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else Debug.LogError($"{this.gameObject} 的物品槽持有者为空");
    }
    public override void AssignSlot(InventorySystem inventorySystem)
    {
        slotForUI = new SerializableDictionary<InventorySlotForUI, InventorySlot>();
        if(inventorySlotForUI.Length != inventorySystem.InventorySize) 
        {
            Debug.LogError($"{this.gameObject} 的物品槽数量与物品槽持有者的物品槽数量不一致");
            //return;
        }
        for (int i = 0; i < inventorySlotForUI.Length; i++)
        {
            slotForUI.Add(inventorySlotForUI[i], inventorySystem.InventorySlots[i]);
            inventorySlotForUI[i].Init(inventorySystem.InventorySlots[i]);
        }
    }

}
