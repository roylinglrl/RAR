using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EquipmentSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image equipmentIcon;
    [SerializeField] private Image slotBackground;
    [SerializeField] private TextMeshProUGUI equipmentNameText;
    [SerializeField] private TextMeshProUGUI slotTypeText;
    [SerializeField] private Button slotButton;
    
    [Header("Colors")]
    [SerializeField] private Color emptySlotColor = Color.gray;
    [SerializeField] private Color filledSlotColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;
    
    public EquipmentType EquipmentType { get; private set; }
    public int SlotIndex { get; private set; }
    public EquipmentInstance CurrentEquipment { get; private set; }
    public event Action<EquipmentSlotUI> OnSlotClicked;
    
    private EquipmentDisplay parentDisplay;
    
    public void Initialize(EquipmentType equipmentType, int slotIndex = 0)
    {
        EquipmentType = equipmentType;
        SlotIndex = slotIndex;
        
        if (slotTypeText != null)
            slotTypeText.text = GetEquipmentTypeName(equipmentType);
        
        if (slotButton != null)
            slotButton.onClick.AddListener(OnButtonClicked);
        
        parentDisplay = GetComponentInParent<EquipmentDisplay>();
        
        ClearSlot();
    }
    
    public void RefreshSlot(EquipmentInstance equipment)
    {
        CurrentEquipment = equipment;
        
        if (equipment != null && equipment.ItemData != null)
        {
            // 有装备
            equipmentIcon.sprite = equipment.ItemData.ItemIcon;
            equipmentIcon.color = filledSlotColor;
            
            if (equipmentNameText != null)
                equipmentNameText.text = equipment.ItemData.ItemName;

                UpdateQualityDisplay(equipment);
        }
        else
        {
            // 空槽位
            ClearSlot();
        }
    }
        private void UpdateQualityDisplay(EquipmentInstance equipment)
    {
        // 这里可以根据装备品质设置不同的显示效果
        // 例如：改变边框颜色、添加特效等
    }
    
    public void ClearSlot()
    {
        CurrentEquipment = null;
        equipmentIcon.sprite = null;
        equipmentIcon.color = emptySlotColor;
        
        if (equipmentNameText != null)
            equipmentNameText.text = "";
    }
    
    private void OnButtonClicked()
    {
        OnSlotClicked?.Invoke(this);
    }
    
    private string GetEquipmentTypeName(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Armor: return "装甲";
            case EquipmentType.EnergyModule: return "能源插件";
            case EquipmentType.Core: return "核心";
            case EquipmentType.Processor: return "处理器";
            case EquipmentType.Backpack: return "背包";
            case EquipmentType.Weapon: return "武器";
            default: return type.ToString();
        }
    }
        public void SetHighlight(bool highlight)
    {
        if (slotBackground != null)
        {
            slotBackground.color = highlight ? highlightColor : emptySlotColor;
        }
    }
}