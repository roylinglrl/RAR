using Mono.Cecil;
using UnityEngine;

public enum AttributeType
{
    MaxHealth,
    AttackDamage,
    CriticalChance,
    Armor,
    MovementSpeed,
}
[System.Serializable]
public class Attribute
{
    public AttributeType type;
    public float baseValue;

    public Attribute(AttributeType type, float value)
    {
        this.type = type;
        this.baseValue = value;
    }
}
public class AttributeModifier 
{
    public AttributeType type;
    public float value;
    public ModifierType modifierType;
    public object source; // 修饰符来源（武器、装备、配件等）

    public AttributeModifier(AttributeType type, float value, ModifierType modifierType, object source = null)
    {
        this.type = type;
        this.value = value;
        this.modifierType = modifierType;
        this.source = source;
    }
}
public enum ModifierType
{
    Flat,       // 固定值加成：+10
    PercentAdd, // 百分比加成：+10%
    PercentMult // 百分比乘算：×1.1
}
