using UnityEngine;

public interface IDamageModifier
{
    int Priority { get; }
    void OnPreHit(DamageInfo damageInfo);//预处理
    void OnCalculateDamage(DamageInfo damageInfo);//计算伤害
    void OnPostHit(DamageInfo damageInfo);//后处理
}
