using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character System/Character")]
	public class CharacterSO: ScriptableObject
	{
		//角色的各项数据
		public String CharacterName;//角色名称
		public String CharacterID;//角色ID
		public String CharacterDesc;//角色描述
		public String CharacterType;//角色类型
		public int CharacterMaxHealth;//角色最大生命值
		public int CharacterMoveSpeed;//角色移动速度
	}
