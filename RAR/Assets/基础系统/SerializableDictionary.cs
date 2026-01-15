using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        if (keys.Count != values.Count)
        {
            Debug.LogError($"[SerializableDictionary] 键值对数量不匹配: 键数量={keys.Count}, 值数量={values.Count}");
            return;
        }
        for (int i = 0; i < keys.Count; i++)
        {
            // 检查是否已存在相同键，避免重复添加
            if (!this.ContainsKey(keys[i]))
            {
                this.Add(keys[i], values[i]);
            }
            else
            {
                Debug.LogWarning($"[SerializableDictionary] 发现重复键: {keys[i]}, 跳过重复项");
            }
        }
    }
}
