using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CurrentItemData : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;

    public InventorySlot AssignedInventorySlot;
    public InventorySlot originalInventorySlot;
    public InventorySlotForUI originalInventorySlotForUI;

    public InventoryDisplay parentDisplay;

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemCount.text = "";
    }
    public void UpdateItemSlot(InventorySlot newInventorySlot, InventorySlotForUI newInventorySlotForUI)
    {
        AssignedInventorySlot.AssignItem(newInventorySlot);//更新当前物品槽的物品数据
        originalInventorySlot = newInventorySlot;
        originalInventorySlotForUI = newInventorySlotForUI;
        ItemSprite.sprite = newInventorySlot.ItemInstance.ItemData.ItemIcon;//更新当前显示的物品图标
        ItemCount.text = newInventorySlot.ItemCount.ToString();//更新当前显示的物品数量
        ItemSprite.color = Color.white;//显示物品图标
    }
    void Update()
    {
        if(AssignedInventorySlot.ItemInstance != null)
        {
            transform.position = Mouse.current.position.ReadValue();//更新当前物品数据的位置
            if(Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI())
            {
                if (TryDropOnEquipmentSlot())
                {
                    // 如果点击在装备槽上，装备系统会处理
                    return;
                }
                if (originalInventorySlotForUI != null)
                {
                    originalInventorySlot.AssignItem(AssignedInventorySlot);
                    originalInventorySlotForUI.UpdateSlotUI();
                }
                if(originalInventorySlotForUI == null)
                {
                    return;
                }
                CloseSlot();
            }
        }
    }
        private bool TryDropOnEquipmentSlot()
    {
        // 获取鼠标位置下的所有UI元素
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        // 检查是否有装备槽
        foreach (var result in results)
        {
            var equipmentSlotUI = result.gameObject.GetComponent<EquipmentSlotUI>();
            if (equipmentSlotUI != null)
            {
                // 如果点击在装备槽上，返回true
                // EquipmentDisplay会处理点击事件
                return true;
            }
        }
        
        return false;
    }
        // 添加一个方法用于从装备更新物品
    public void UpdateItemSlotFromEquipment(InventorySlot newInventorySlot)
    {
        AssignedInventorySlot.AssignItem(newInventorySlot);
        
        if (newInventorySlot.ItemInstance != null && newInventorySlot.ItemInstance.ItemData != null)
        {
            ItemSprite.sprite = newInventorySlot.ItemInstance.ItemData.ItemIcon;
            ItemCount.text = newInventorySlot.ItemCount.ToString();
            ItemSprite.color = Color.white;
        }
        else
        {
            CloseSlot();
        }
    }
    public void ClearSlot()
    {
        AssignedInventorySlot.ClearSlot();//清空当前物品槽
    }
    public void CloseSlot()
    {
        AssignedInventorySlot.ClearSlot();//关闭当前物品槽
        ItemSprite.color = Color.clear;//隐藏物品图标
        ItemCount.text = "";//清空物品数量
        ItemSprite.sprite = null;//清空物品图标
    }
    public static bool IsPointerOverUI()
    {
        PointerEventData eventPosition = new PointerEventData(EventSystem.current);
        eventPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventPosition, results);
        return results.Count > 0;
    }
}
