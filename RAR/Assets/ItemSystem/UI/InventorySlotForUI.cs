using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class InventorySlotForUI : MonoBehaviour
{
    [SerializeField]private Image ItemSprite;
    [SerializeField]private TextMeshProUGUI ItemCount;
    [SerializeField]private InventorySlot assignedInventorySlot;

    private Button button;
    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentInventoryDisplay { get; private set; }//父物品槽
    private void Awake()
    {
        ClearSlot();
        button = this.GetComponent<Button>();
        button.onClick.AddListener(OnUISlotClick);
        ParentInventoryDisplay = transform.parent.GetComponent<InventoryDisplay>();
    }
    public void Init(InventorySlot inventorySlot)
    {
        assignedInventorySlot = inventorySlot;
        UpdateSlotUI(inventorySlot);
    }
    public void UpdateSlotUI(InventorySlot inventorySlot)
    {
        if(inventorySlot.ItemInstance != null)
        {
            ItemSprite.sprite = inventorySlot.ItemInstance.ItemData.ItemIcon;
            ItemSprite.color = Color.white;
            if(inventorySlot.ItemCount > 0) ItemCount.text = inventorySlot.ItemCount.ToString();
            else ItemCount.text = "";
        }
        else
        {
            ClearSlot();
        }
    }
    public void UpdateSlotUI()
    {
        if(assignedInventorySlot == null) return;
        UpdateSlotUI(assignedInventorySlot);
    }
    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();
        ItemSprite.sprite = null;
        ItemSprite.color = Color.clear;
        ItemCount.text = "";
    }
    public void OnUISlotClick()
    {
        //TODO: 点击物品槽时的操作
        ParentInventoryDisplay?.SlotClicked(this);
    }
}
