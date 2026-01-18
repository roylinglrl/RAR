
using UnityEngine;

public class ItemSO : ScriptableObject//物品数据脚本
{
    public int itemID;//物品ID
    public string itemName;//物品名称
    public Sprite itemIcon;//物品图标
    public int itemPrice;//物品价格
    public int itemStack;//物品最大堆叠数量
    public ItemType itemType;//物品类型
    public EquipmentType equipmentType;//装备类型
    public SerializableDictionary<string, float> itemAttributes = new SerializableDictionary<string, float>();//物品属性
}

public enum ItemType
{
    None,//无类型
    Weapon,//武器
    Armor,//护甲
    Consumable,//消耗品
    Material,//材料
    QuestItem,//任务物品
    KeyItem,//关键物品
}
public enum EquipmentType
{
    None,//无装备类型
    Gun,//枪类武器
    Melee,//近战武器
    
    // 装备类型
    Head,//头装备
    Body,//身体装备
    Hands,//手装备
    Feet,//脚装备
    Backpack,//背包装备
    Accessory,//附件装备
}