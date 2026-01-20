using UnityEngine;

public class DebugInput : MonoBehaviour
{
    public ItemData itemData;
    public ItemData itemData2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            GetComponent<InventoryHolder>().InventorySystem.AddToInventory(itemData, 1);
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<InventoryHolder>().InventorySystem.AddToInventory(itemData2, 1);
        }
    }
}
