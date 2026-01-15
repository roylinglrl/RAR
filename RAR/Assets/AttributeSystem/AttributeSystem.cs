using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AttributeSystem : MonoBehaviour
{
    private SerializableDictionary<AttributeType,float> baseAttributes = new SerializableDictionary<AttributeType,float>();
    private List<AttributeModifier> allModifiers = new List<AttributeModifier>();

    private SerializableDictionary<AttributeType,float> finalAttributes = new SerializableDictionary<AttributeType,float>();

    private bool isDirty = true;

    public void InitializeBaseAttributes(List<Attribute> modifiers)
    {
        baseAttributes.Clear();
        foreach (var attr in modifiers)
        {
            baseAttributes[attr.type] = attr.baseValue;
        }
    }
}
