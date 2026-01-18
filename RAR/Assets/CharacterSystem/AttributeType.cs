using System;
using UnityEngine;

public enum AttributeType
{
    //角色的属性类型
    MaxHealth,//最大生命值 从角色获取
    MaxShield,//最大护盾值 从角色获取
    ShieldDamageReductionRatio,//护盾值伤害减免比率 从角色获取 不会超过1
    Armor,//护甲值 从角色获取
    MaxEnergy,//最大能量值 从角色获取
    SpeedMovement,//移动速度 从角色获取
    MaxStamina,//最大耐力值 从角色获取
    WeightCapacity,//负重能力 从角色获取
    BackPackCapacity,//背包容量 从角色及装备的背包获取
    WeightReductionRatio,//负重减免比率 从角色装备的背包获取 不会超过1
    SafeBackPackCapacity,//安全背包容量 从角色及装备的背包获取
    CoolDownReductionRatio,//冷却缩减比率 从角色获取 不会超过1

    //武器的属性类型
    Damage,//伤害值 从武器获取
    CriticalChance,//暴击率 从武器获取 不会超过1
    CriticalDamageMultiplier,//暴击伤害倍数 从武器获取
    AmmoCapacity,//弹药容量 从武器获取
    ReloadSpeed,//换弹速度 从武器获取
    FireRate,//射速 从武器获取
    WeaponRange,//武器射程 从武器获取
    ShootBulletCount,//武器每次射击子弹数量 从武器获取
    ScatterAngle,//武器散射角度 从武器获取
    ArmorPenetrationRatio,//武器护甲穿透比率 从武器获取 不会超过1
    ArmorPenetrationFlat,//武器护甲穿透固定值 从武器获取
}