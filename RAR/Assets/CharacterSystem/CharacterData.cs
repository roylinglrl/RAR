using System;
using UnityEngine;

[Serializable]//可序列化类
public class CharacterData
{
    public String CharacterID { get; set; }//角色ID, 唯一标识符 从CharacterSO中获取静态数据
    public String CharacterName { get; set; }//角色名称 从CharacterSO中获取静态数据
    public int CurrentLevel { get; set; } = 1;//当前等级
    public float CurrentExperience { get; set; } = 0f;//当前经验值
    public bool isHurt = false;//是否受伤

    [NonSerialized]
    private CharacterSO CharacterSO;//角色的ScriptableObject数据
    public CharacterSO GetCharacterSO()//获取CharacterSO
    {
        if (CharacterSO == null)
        {
            CharacterSO = Resources.Load<CharacterSO>("CharacterSO/" + CharacterID);
            if (CharacterSO == null)
            {
                Debug.LogError("CharacterSO not found for ID: " + CharacterID);
            }
        }
        return CharacterSO;
    }
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