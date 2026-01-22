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
    protected virtual void Start()
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
    public void SlotClicked(InventorySlotForUI clickedUISlot)
    {
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;


        if(clickedUISlot.AssignedInventorySlot.ItemInstance != null &&
        currentItemData.AssignedInventorySlot.ItemInstance == null)
        {
            if(isShiftPressed && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot splitSlot))
            {
                currentItemData.UpdateItemSlot(splitSlot);//更新当前显示的物品数据
                clickedUISlot.UpdateSlotUI();//更新点击的物品槽
                return;
            }else//如果没有按下Shift键
            {
                currentItemData.UpdateItemSlot(clickedUISlot.AssignedInventorySlot);//更新当前显示的物品数据   
                clickedUISlot.ClearSlot();//清空点击的物品槽
                return;
            }
        }
        if(clickedUISlot.AssignedInventorySlot.ItemInstance == null &&
        currentItemData.AssignedInventorySlot.ItemInstance != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(currentItemData.AssignedInventorySlot);//更新点击的物品槽
            clickedUISlot.UpdateSlotUI();//更新点击的物品槽
            currentItemData.CloseSlot();//关闭当前物品槽
            return;
        }
        if(clickedUISlot.AssignedInventorySlot.ItemInstance != null &&
        currentItemData.AssignedInventorySlot.ItemInstance != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemInstance.ItemData == currentItemData.AssignedInventorySlot.ItemInstance.ItemData;
            if(isSameItem
                && clickedUISlot.AssignedInventorySlot.RoomLeftInStack(currentItemData.AssignedInventorySlot.ItemCount)
            )
            {
                int currentCount = currentItemData.AssignedInventorySlot.ItemCount;
                clickedUISlot.AssignedInventorySlot.AddToStack(currentCount);//将物品添加到物品槽的栈中
                clickedUISlot.UpdateSlotUI();//更新点击的物品槽
                currentItemData.CloseSlot();//清空当前物品槽
            }
            else if(isSameItem && !clickedUISlot.AssignedInventorySlot.RoomLeftInStack(currentItemData.AssignedInventorySlot.ItemCount, out int leftStack))
            {
                if(leftStack < 1) SwapSlots(clickedUISlot);//交换物品槽 如果物品槽没有空间了
                else //如果物品槽有空间
                {
                    int remain = currentItemData.AssignedInventorySlot.ItemCount - leftStack;
                    clickedUISlot.AssignedInventorySlot.AddToStack(leftStack);//将物品添加到物品槽的栈中
                    clickedUISlot.UpdateSlotUI();//更新点击的物品槽
                    var newItem = new InventorySlot(currentItemData.AssignedInventorySlot.ItemInstance, remain);//创建新物品
                    currentItemData.CloseSlot();//清空当前物品槽
                    currentItemData.UpdateItemSlot(newItem);//更新当前显示的物品数据
                    
                }
            }

            else if(!isSameItem)
            {
                SwapSlots(clickedUISlot);//交换物品槽
            }
        }
    }
        private void SwapSlots(InventorySlotForUI clickedUISlot)
    {
        var clone = new InventorySlot(currentItemData.AssignedInventorySlot.ItemInstance, currentItemData.AssignedInventorySlot.ItemCount);
        currentItemData.CloseSlot();
        currentItemData.UpdateItemSlot(clickedUISlot.AssignedInventorySlot);//更新当前显示的物品数据
        clickedUISlot.AssignedInventorySlot.AssignItem(clone);//更新点击的物品槽
        clickedUISlot.UpdateSlotUI();//更新点击的物品槽
    }
}
