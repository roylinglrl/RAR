
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character System/Character")]
	public class CharacterSO: ScriptableObject
	{
		//角色的各项数据
		public String CharacterName;//角色名称
		public String CharacterID;//角色ID
		public String CharacterDesc;//角色描述
		public String CharacterType;//角色类型
		//角色的外观
		public Sprite CharacterIcon;//角色图标
		public GameObject CharacterPrefab;//角色预制体

		//角色的成长
		public int MaxLevel = 90;
		/// 角色的基础属性值
		public float MaxHealth = 100f;//最大生命值
		public float MaxShield = 50f;//最大护盾值
		[Range(0.0f, 1.0f)]
		public float ShieldDamageReductionRatio = 0.5f;//护盾值伤害减免比率 不会超过1
		public float Armor = 10f;//护甲值
		public float MaxEnergy = 100f;//最大能量值
		public float SpeedMovement = 5f;//移动速度
		public float MaxStamina = 100f;//最大耐力值
		public float WeightCapacity = 100f;//负重能力
		public float BackPackCapacity = 10f;//背包容量(初始)
		[Range(0.0f, 1.0f)]
		public float WeightReductionRatio = 0.0f;//负重减免比率 不会超过1
		public float SafeBackPackCapacity = 2f;//安全背包容量(初始)
		[Range(0.0f, 1.0f)]
		public float CoolDownReductionRatio = 0.0f;//冷却缩减比率 不会超过1

		//角色的技能 TODO: 添加技能数据结构



	}
