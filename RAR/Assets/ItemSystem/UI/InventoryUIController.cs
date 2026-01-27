using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay chestPanel;
    public DynamicInventoryDisplay playerBackpackPanel;
            [Header("装备栏")]
    [SerializeField] private EquipmentDisplay equipmentDisplay;

    private ChestInventory currentChest;

    void Awake()
    {
        chestPanel.gameObject.SetActive(false);
        playerBackpackPanel.gameObject.SetActive(false);
        if (equipmentDisplay != null)
            equipmentDisplay.gameObject.SetActive(false);
    }
    private void OnEnable()//启用时订阅事件
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayChestInventory;
        PlayerBackpackHolder.OnPlayerBackpackDisplayRequested += DisplayPlayerBackpack;
    }
    private void OnDisable()//禁用时取消订阅事件
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayChestInventory;
        PlayerBackpackHolder.OnPlayerBackpackDisplayRequested -= DisplayPlayerBackpack;
    }
    void Update()
    {
        if(chestPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            chestPanel.gameObject.SetActive(false);
            HideEquipmentPanel();
            GameManager.Instance.IsOnUI = false;
            if (currentChest != null)
            {
                currentChest.NotifyInteractionCompleted();
                currentChest = null; // 清空引用
            }
        }
        if(playerBackpackPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            playerBackpackPanel.gameObject.SetActive(false);
            HideEquipmentPanel();
            GameManager.Instance.IsOnUI = false;
        }
    }
    private void DisplayChestInventory(InventorySystem inventoryToDisplay,InventoryHolder chestHolder)
    {
        currentChest = chestHolder as ChestInventory;
        chestPanel.gameObject.SetActive(true);
        chestPanel.RefreshDynamicInventoryDisplay(inventoryToDisplay);
        PlayerManager.Instance.PlayerBackpackHolder.ReadyBackpack();
        playerBackpackPanel.gameObject.SetActive(true);
        playerBackpackPanel.RefreshDynamicInventoryDisplay(PlayerManager.Instance.PlayerBackpackHolder.BackpackInventorySystem);
        ShowEquipmentPanel();
    }
    private void DisplayPlayerBackpack(InventorySystem inventoryToDisplay)
    {
        playerBackpackPanel.gameObject.SetActive(true);
        playerBackpackPanel.RefreshDynamicInventoryDisplay(inventoryToDisplay);
        ShowEquipmentPanel();
    }
        private void ShowEquipmentPanel()
    {
        if (equipmentDisplay != null)
        {
            equipmentDisplay.gameObject.SetActive(true);
        }
    }
        private void HideEquipmentPanel()
    {
        // 只有当所有库存面板都关闭时才关闭装备面板
        if (!chestPanel.gameObject.activeInHierarchy && 
            !playerBackpackPanel.gameObject.activeInHierarchy)
        {
            if (equipmentDisplay != null)
                equipmentDisplay.gameObject.SetActive(false);
        }
    }
    
}