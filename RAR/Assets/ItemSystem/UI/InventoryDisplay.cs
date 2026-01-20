using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] CurrentItemData currentItemData;//当前显示的物品数据
    protected InventorySystem inventorySystem;//当前显示的物品槽
    protected SerializableDictionary<InventorySlotForUI, InventorySlot> slotForUI;//UI物品槽
    public InventorySystem InventorySystem => inventorySystem;//当前显示的物品槽
    public SerializableDictionary<InventorySlotForUI, InventorySlot> SlotForUI => slotForUI;//UI物品槽
    public virtual void Start()
    {
        
    }
    public abstract void AssignSlot(InventorySystem inventoryToDisplay);//分配物品槽
    protected virtual void UpdateSlot(InventorySlot updateSlot)//更新物品槽
    {
        foreach (var slot in slotForUI)//遍历UI物品槽
        {
            if (slot.Value == updateSlot)//如果物品槽与更新的物品槽相同
            {
                slot.Key.UpdateSlotUI(updateSlot);//更新UI物品槽
            }
        }
    }
    public void SlotClicked(InventorySlotForUI clickedSlot)
    {
        Debug.Log("点击了物品槽:" + clickedSlot.AssignedInventorySlot.ItemData.ItemName);
    }
}
