using System;
using UnityEngine;

[Serializable]//可序列化类
public class CharacterData
{
    public String CharacterID { get; set; }//角色ID, 唯一标识符 从CharacterSO中获取静态数据
    public String CharacterName { get; set; }//角色名称 从CharacterSO中获取静态数据
    
    [NonSerialized]
    public CharacterSO CharacterSO;//角色的ScriptableObject数据

    public CharacterData(String characterID)//角色数据构造函数
    {
        CharacterID = characterID;
        Initialize();
    }

    public void Initialize()//从CharacterSO初始化角色数据
    {
        GetCharacterSO(CharacterID);
        if (CharacterSO == null)
        {
            Debug.LogError("CharacterSO not found for ID: " + CharacterID);
            return;
        }
        CharacterName = CharacterSO.CharacterName;
    }
    public void GetCharacterSO(String characterID)//根据ID获取CharacterSO
    {
        CharacterSO = Resources.Load<CharacterSO>("CharacterSO/" + characterID);
        if (CharacterSO == null)
        {
            Debug.LogError("CharacterSO not found for ID: " + characterID);
        }
        else
        {
            CharacterID = CharacterSO.CharacterID;
        }
    }
}