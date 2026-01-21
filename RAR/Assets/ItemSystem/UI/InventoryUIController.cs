using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay inventoryPanel;
    void Awake()
    {
        inventoryPanel.gameObject.SetActive(false);
    }
    private void OnEnable()//启用时订阅事件
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
    }
    private void OnDisable()//禁用时取消订阅事件
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
    }
    void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (!CharacterManager.Instance.attributeManager.ContainsAttribute(AttributeType.BackPackCapacity))
            {
                Debug.LogError("未包含BackPackCapacity属性，可能是属性尚未初始化");
                return;
            }
            DisplayInventory(new InventorySystem((int) Math.Floor(CharacterManager.Instance.attributeManager.GetFinalAttributeValue(AttributeType.BackPackCapacity))));
        }
         if(inventoryPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            inventoryPanel.gameObject.SetActive(false);
        }
    }
    private void DisplayInventory(InventorySystem inventoryToDisplay)
    {
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.RefreshDynamicInventoryDisplay(inventoryToDisplay);
    }

}