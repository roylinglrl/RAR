using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item System/Item")]
public class ItemData : ScriptableObject
{
    //物品的各项数据
    public String ItemName;//物品名称
    public String ItemID;//物品ID
    [TextArea(4, 10)]public String ItemDesc;//物品描述
    public int MaxStack;//物品最大堆叠数量
    public Sprite ItemIcon;//物品图标

}