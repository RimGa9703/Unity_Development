
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();


    [SerializeField]
    private List<TValue> values = new List<TValue>();


    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }


    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError($"[SerializableDictionary] Key/Value 개수가 다릅니다. Keys: {keys.Count}, Values: {values.Count}");
            return; // 또는 적절한 fallback 처리
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this[keys[i]] = values[i]; // 중복 키 방지
        }
    }
}