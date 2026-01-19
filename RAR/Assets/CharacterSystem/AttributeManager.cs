using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class AttributeManager //属性管理器
{
    public Dictionary<AttributeType, float> BaseAttributes = new Dictionary<AttributeType, float>();//基础属性
    public List<AttributeModifier> Modifiers = new List<AttributeModifier>();//属性修改器

    public bool _isDirty = true;//是否需要重新计算属性

    private Dictionary<AttributeType, float> _cachedFinalAttributes = new Dictionary<AttributeType, float>();//缓存的最终属性值

    public AttributeManager()//属性管理器构造函数
    {
        BaseAttributes = new Dictionary<AttributeType, float>();
        Modifiers = new List<AttributeModifier>();
        _isDirty = true;
        _cachedFinalAttributes = new Dictionary<AttributeType, float>();
    }

    public void initializeDefaultAttributes()//初始化默认属性
    {
        //TODO: 从CharacterSO中获取默认属性值
        CharacterSO characterSO = CharacterManager.Instance.currentCharacterData.GetCharacterSO();
        if (characterSO == null)
        {
            Debug.LogError("CharacterSO not found for ID: " + CharacterManager.Instance.currentCharacterData.CharacterID);
            return;
        }
        BaseAttributes.Clear();
        BaseAttributes[AttributeType.MaxHealth] = characterSO.MaxHealth;//最大生命值
        BaseAttributes[AttributeType.MaxShield] = characterSO.MaxShield;//最大护盾值
        BaseAttributes[AttributeType.ShieldDamageReductionRatio] = characterSO.ShieldDamageReductionRatio;//护盾值伤害减免比率 不会超过1
        BaseAttributes[AttributeType.Armor] = characterSO.Armor;//护甲值
        BaseAttributes[AttributeType.MaxEnergy] = characterSO.MaxEnergy;//最大能量值
        BaseAttributes[AttributeType.SpeedMovement] = characterSO.SpeedMovement;//移动速度
        BaseAttributes[AttributeType.MaxStamina] = characterSO.MaxStamina;//最大耐力值
        BaseAttributes[AttributeType.WeightCapacity] = characterSO.WeightCapacity;//负重能力
        BaseAttributes[AttributeType.BackPackCapacity] = characterSO.BackPackCapacity;//背包容量(初始)
        BaseAttributes[AttributeType.WeightReductionRatio] = characterSO.WeightReductionRatio;//负重减免比率 不会超过1
        BaseAttributes[AttributeType.SafeBackPackCapacity] = characterSO.SafeBackPackCapacity;//安全背包容量(初始)
        BaseAttributes[AttributeType.CoolDownReductionRatio] = characterSO.CoolDownReductionRatio;//冷却缩减比率 不会超过1
        //TODO : 初始化其他属性
        MarkDirty();
    }
    private void MarkDirty()//标记属性为脏，需要重新计算
    {
        _isDirty = true;
    }
    public void RecalculateIfDirty()//如果属性为脏，重新计算
    {
        if (!_isDirty) return;
        _cachedFinalAttributes.Clear();
        foreach (var attributeType in BaseAttributes.Keys)
        {
            var finalValue = CalculateFinalValue(attributeType);
            _cachedFinalAttributes[attributeType] = finalValue;
        }
        _isDirty = false;
    }
    public float CalculateFinalValue(AttributeType attributeType)//计算最终属性值
    {
        //如果属性未初始化，返回0
        if (!BaseAttributes.TryGetValue(attributeType, out var baseValue))
        {
            Debug.LogError($"属性 {attributeType} 未初始化");
            return 0f;
        }
        //计算属性的最终值，包括基础值和所有修改器
        var modifiersForAttribute =
            Modifiers.Where(modifier => modifier.AttributeType == attributeType).ToList();
        //如果没有修改器，返回基础值
        if (modifiersForAttribute.Count == 0)
        {
            return baseValue;
        }
        float finalValue = baseValue;//最终属性值，初始为基础值
        //计算所有加法修改器的总和
        float additiveSum = modifiersForAttribute
                        .Where(modifier => modifier.ModifierType == ModifierType.Additive)
            .Sum(modifier => modifier.ModifierValue);
        finalValue += additiveSum;//将加法修改器的总和加到最终值上
        //计算所有乘法修改器的乘积
        var multiplicativeModifiers = modifiersForAttribute
                        .Where(modifier => modifier.ModifierType == ModifierType.Multiplicative).ToList();
        //计算所有加法堆叠修改器的乘积
        foreach (var modifier in multiplicativeModifiers)
        {
            finalValue += modifier.ModifierValue * baseValue;
        }
        // 计算所有加法堆叠修改器的乘积
        var stackingModifiers = modifiersForAttribute
                        .Where(modifier => modifier.ModifierType == ModifierType.Adding_stacking).ToList();
        float stackingProduct = 1f;
        //计算所有加法堆叠修改器的乘积
        foreach (var modifier in stackingModifiers)
        {
            stackingProduct *= (1 + modifier.ModifierValue);
        }
        //将所有加法堆叠修改器的乘积乘到最终值上
        finalValue *= stackingProduct;
        return finalValue;//返回最终属性值
    }
    public float GetFinalAttributeValue(AttributeType attributeType)//获取最终属性值
    {
        RecalculateIfDirty();
        if (_cachedFinalAttributes.TryGetValue(attributeType, out var finalValue))
        {
            return finalValue;
        }
        Debug.LogError($"属性 {attributeType} 未计算");
        return 0f;
    }
    public Dictionary<AttributeType, float> GetAllFinalAttributes()//获取所有最终属性值
    {
        RecalculateIfDirty();
        return new Dictionary<AttributeType, float>(_cachedFinalAttributes);
    }
    public void SetBaseAttribute(AttributeType attributeType, float value)//设置基础属性值
    {
        BaseAttributes[attributeType] = value;
        MarkDirty();
    }
    public void addModifier(AttributeModifier modifier)//添加属性修改器
    {
        Modifiers.Add(modifier);
        _isDirty = true;
        MarkDirty();
    }
    public void removeModifierById(string id)//根据ID移除修改器
    {
        Modifiers.RemoveAll(modifier => modifier.Id == id);
        _isDirty = true;
        MarkDirty();
    }
    public void removeModifierBySource(string source)//根据来源移除修改器
    {
        Modifiers.RemoveAll(modifier => modifier.Source == source);
        _isDirty = true;
        MarkDirty();
    }
    public void removeAllModifiers()//移除所有修改器
    {
        Modifiers.Clear();
        _isDirty = true;
        MarkDirty();
    }
}
public enum ModifierType
{
    Additive,//加法修改器
    Multiplicative,//乘法修改器
    Adding_stacking,//加法堆叠修改器
}
[Serializable]
public class AttributeModifier
{
    public AttributeType AttributeType { get; private set; }//属性类型
    public float ModifierValue { get; private set; }//修改器值
    public ModifierType ModifierType { get; set; }//修改器类型
    public string Source { get; set; }//修改器来源
    public string Id { get; set; }//修改器ID
    public AttributeModifier(AttributeType attributeType, float modifierValue,ModifierType modifierType,string source,string id)//属性修改器构造函数
    {
        AttributeType = attributeType;
        ModifierValue = modifierValue;
        ModifierType = modifierType;
        Source = source;
        Id = id;
    }
}
