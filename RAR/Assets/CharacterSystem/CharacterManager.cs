using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public CharacterData currentCharacterData;//当前角色数据 从CharacterData中获取
    public List<CharacterData> UnlockedCharacters = new List<CharacterData>();//已解锁角色列表
    //public AttributeManager attributeManager;//属性管理器引用
    public void AddNewCharacter(String characterID)//添加新角色到已解锁列表

    {
        CharacterSO characterSO = Resources.Load<CharacterSO>("CharacterSO/" + characterID);
        if (characterSO == null)
        {
            Debug.LogError("CharacterSO not found: " + characterID);
            return;
        }
        if (UnlockedCharacters.Find(character => character.CharacterID == characterID) != null)
        {
            Debug.Log("Character already unlocked: " + characterID);
            //TODO: 处理角色已解锁的情况
            return;
        }
        CharacterData newCharacterData = new CharacterData(characterID);
        UnlockedCharacters.Add(newCharacterData);
    }
    public void AddNewCharacter(CharacterSO characterSO)//添加新角色到已解锁列表
    {
        if (characterSO == null)
        {
            Debug.LogError("CharacterSO is null.");
            return;
        }
        if (UnlockedCharacters.Find(character => character.CharacterID == characterSO.CharacterID) != null)
        {
            Debug.Log("Character already unlocked: " + characterSO.CharacterID);
            //TODO: 处理角色已解锁的情况
            return;
        }
        CharacterData newCharacterData = new CharacterData(characterSO);
        UnlockedCharacters.Add(newCharacterData);
    }
    public void RemoveCharacter(String characterID)//从已解锁列表移除角色
    {
        CharacterData characterToRemove = UnlockedCharacters.Find(character => character.CharacterID == characterID);
        if (characterToRemove != null)
        {
            UnlockedCharacters.Remove(characterToRemove);
        }
        else
        {
            Debug.Log("Character not found in unlocked list: " + characterID);
        }
    }
    public CharacterData GetCharacterData(String characterID)//根据ID获取角色数据
    {
        return UnlockedCharacters.Find(character => character.CharacterID == characterID);
    }
    public void SetCurrentCharacter(String characterID)//设置当前角色
    {
        CharacterData characterData = GetCharacterData(characterID);
        if (characterData != null)
        {
            currentCharacterData = characterData;
        }
        else
        {
            Debug.LogError("Character not found in unlocked list: " + characterID);
        }
        PlayerManager.Instance.PlayerCombatEntity.attributeManager.initializeDefaultAttributes();
    }
    public void AddExperience(float experience)//为当前角色添加经验值
    {
        if (currentCharacterData == null)
        {
            Debug.LogError("No current character set.");
            return;
        }
        currentCharacterData.CurrentExperience += experience;
        PlayerManager.Instance.PlayerCombatEntity.attributeManager.MarkDirty();
    }

}