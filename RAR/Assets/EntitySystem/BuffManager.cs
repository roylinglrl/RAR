using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffManager : MonoBehaviour
{
    public List<Buff> activeBuffs = new List<Buff>();
    public List<T> GetBuffs<T>() where T : class
    {
        List<T> result = new List<T>();
        foreach (var buff in activeBuffs)
        {
            if (buff is T typedBuff)
            {
                result.Add(typedBuff);
            }
        }
        return result;
    }
        public void AddBuff(Buff buff)
    {
        activeBuffs.Add(buff);
    }

    public void RemoveBuff(Buff buff)
    {
        activeBuffs.Remove(buff);
    }

}