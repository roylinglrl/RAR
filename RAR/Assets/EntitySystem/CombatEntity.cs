using System.Collections.Generic;
using UnityEngine;

public class CombatEntity : MonoBehaviour
{
    public EntityType entityType;
    public float maxHealth;
    public float currentHealth;
    public AttributeManager attributeManager;
    public BuffManager buffManager;
    void Awake() {
        buffManager = GetComponent<BuffManager>();
        attributeManager = new AttributeManager();
    }
    public void ModifyHealth(float amount)
    {   
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }
        public virtual void OnDeath()
    {
        Debug.Log("Entity " + name + " has died.");
        //TODO: 处理实体死亡的逻辑
        if (entityType == EntityType.Player)
        {
            // 玩家死亡，游戏结束
            //GameManager.Instance.GameOver();
        }
        else
        {
            // 敌人死亡，奖励玩家经验等
            //TODO: 处理敌人死亡的逻辑
        }
    }
    public void TakeDamage(DamageInfo info) {
        List<IDamageModifier> modifiers = new List<IDamageModifier>();
        modifiers.AddRange(buffManager.GetBuffs<IDamageModifier>());

        modifiers.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        // 3. 执行管道流程
        foreach (var mod in modifiers) mod.OnPreHit(info);
        if (info.IsCanceled) return;

        info.FinalDamage = info.RawDamage;
        foreach (var mod in modifiers) mod.OnCalculateDamage(info);

        // 4. 真正改动 Entity 里的生命值
        ModifyHealth(-info.FinalDamage);

        // 5. 结算后效果
        foreach (var mod in modifiers) mod.OnPostHit(info);
    }
}

public enum EntityType
{
    Player,
    Enemy,
}
