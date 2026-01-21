using System;
using UnityEngine;

[Serializable]//可序列化类
public class CharacterData
{
    [SerializeField]public String CharacterID;//角色ID, 唯一标识符 从CharacterSO中获取静态数据
    [SerializeField]public String CharacterName;//角色名称 从CharacterSO中获取静态数据
    [SerializeField]public int CurrentLevel = 1;//当前等级
    [SerializeField]public float CurrentExperience = 0f;//当前经验值
    [SerializeField]public bool isHurt = false;//是否受伤

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
    public CharacterData(CharacterSO characterSO)//角色数据构造函数
    {
        CharacterID = characterSO.CharacterID;
        InitializeFromSO(characterSO);
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
    public void InitializeFromSO(CharacterSO characterSO)//从CharacterSO初始化角色数据
    {
        CharacterID = characterSO.CharacterID;
        CharacterName = characterSO.CharacterName;
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