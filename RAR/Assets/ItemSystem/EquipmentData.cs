
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Armor,          // 装甲
    EnergyModule,   // 能源插件
    Core,           // 核心
    Processor,      // 高频处理单元
    Backpack,       // 背包
    Weapon          // 武器
}
[CreateAssetMenu( fileName = "New Equipment", menuName = "Item/Equipment" )]
public class EquipmentData : ItemData
{
    public EquipmentType EquipmentType;  // 装备类型
    public int RequiredLevel = 1; // 装备要求等级
    public float Durability = 100f; // 耐久度（如果有的话）
    
    // 装备特有属性
    public float ArmorValue = 0f; // 护甲值（用于装甲）
    public float EnergyCapacity = 0f; // 能量容量（用于能源插件）
    public float ProcessingSpeed = 0f; // 处理速度（用于处理器）
    public int ExtraSlots = 0; // 额外槽位（用于背包）

    public List<AttributeModifier> AttributeModifiersList = new List<AttributeModifier>();
}