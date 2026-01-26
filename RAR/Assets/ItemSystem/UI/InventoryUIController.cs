using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay chestPanel;
    public DynamicInventoryDisplay playerBackpackPanel;

    void Awake()
    {
        chestPanel.gameObject.SetActive(false);
        playerBackpackPanel.gameObject.SetActive(false);
    }
    private void OnEnable()//启用时订阅事件
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayChestInventory;
        PlayerBackpackHolder.OnPlayerBackpackDisplayRequested += DisplayPlayerBackpack;
        PlayerBackpackHolder.OnPlayerBackpackSizeChanged += RefreshPlayerBackpackUI;
    }
    private void OnDisable()//禁用时取消订阅事件
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayChestInventory;
        PlayerBackpackHolder.OnPlayerBackpackDisplayRequested -= DisplayPlayerBackpack;
         PlayerBackpackHolder.OnPlayerBackpackSizeChanged -= RefreshPlayerBackpackUI;
    }
    void Update()
    {
        if(chestPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            chestPanel.gameObject.SetActive(false);
        }
        if(playerBackpackPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            playerBackpackPanel.gameObject.SetActive(false);
        }
    }
    private void DisplayChestInventory(InventorySystem inventoryToDisplay)
    {
        chestPanel.gameObject.SetActive(true);
        chestPanel.RefreshDynamicInventoryDisplay(inventoryToDisplay);
    }
    private void DisplayChestAndPlayerBackpack(InventorySystem chestInventory,InventorySystem playerBackpackInventory)
    {
        chestPanel.gameObject.SetActive(true);
        chestPanel.RefreshDynamicInventoryDisplay(chestInventory);
        playerBackpackPanel.gameObject.SetActive(true);
        playerBackpackPanel.RefreshDynamicInventoryDisplay(playerBackpackInventory);
    }
    private void DisplayPlayerBackpack(InventorySystem inventoryToDisplay)
    {
        playerBackpackPanel.gameObject.SetActive(true);
        playerBackpackPanel.RefreshDynamicInventoryDisplay(inventoryToDisplay);
    }
        private void RefreshPlayerBackpackUI()
    {
        // 检查背包面板是否激活，如果激活则刷新显示
        if (playerBackpackPanel.gameObject.activeInHierarchy && playerBackpackPanel.InventorySystem != null)
        {
            playerBackpackPanel.RefreshDynamicInventoryDisplay(playerBackpackPanel.InventorySystem);
        }
    }

}