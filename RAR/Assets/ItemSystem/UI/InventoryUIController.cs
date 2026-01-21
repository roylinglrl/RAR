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
            DisplayInventory(new InventorySystem(Random.Range(10, 30)));
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