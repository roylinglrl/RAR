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

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemCount.text = "";
    }
    public void UpdateItemSlot(InventorySlot newInventorySlot)
    {
        AssignedInventorySlot.AssignItem(newInventorySlot);//更新当前物品槽的物品数据
        ItemSprite.sprite = newInventorySlot.ItemData.ItemIcon;//更新当前显示的物品图标
        ItemCount.text = newInventorySlot.ItemCount.ToString();//更新当前显示的物品数量
        ItemSprite.color = Color.white;//显示物品图标
    }
    void Update()
    {
        if(AssignedInventorySlot.ItemData != null)
        {
            transform.position = Mouse.current.position.ReadValue();//更新当前物品数据的位置
            if(Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI())
            {
                CloseSlot();
            }
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
