using System;
using Unity.VisualScripting;
using UnityEngine;

public class DebugInput : MonoBehaviour
{
    public ItemData itemData;
    public ItemData itemData2;
    public CharacterSO characterSO;
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
        if (Input.GetKeyDown(KeyCode.L))
        {
            CharacterManager.Instance.AddNewCharacter(characterSO);
            CharacterManager.Instance.SetCurrentCharacter(characterSO.CharacterID);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            CharacterManager.Instance.attributeManager.addModifier(new AttributeModifier(AttributeType.BackPackCapacity,5,ModifierType.Additive,"test_source","test"));
        }
    }
}
