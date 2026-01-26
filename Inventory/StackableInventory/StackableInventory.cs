using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class StackableInventory<T> where T : GameItem, new()
{
    [SerializeField] protected SerializableDictionary<string, List<T>> inventoryDict = new();
    [SerializeField] protected int inventorySize = 5;

    public T this[string code] => FindFirst(code);

    public int Count => inventoryDict.Sum(kv => kv.Value.Count);

    public int InventorySize
    {
        get => inventorySize;
        set => inventorySize = value;
    }

    public virtual T Add(T item)
    {
        if (!inventoryDict.TryGetValue(item.code, out var stacks))
        {
            stacks = new List<T>();
            inventoryDict[item.code] = stacks;
        }

        float remaining = item.count;

        // 기존 스택에 채우기
        foreach (var stack in stacks)
        {
            float space = stack.maxStackCount - stack.count;
            if (space <= 0f) continue;

            int addCount = (int)Mathf.Min(space, remaining);
            stack.count += addCount;
            remaining -= addCount;
            if (remaining <= 0f) break;
        }

        // 초과 수량은 새로운 스택 생성
        while (remaining > 0f)
        {
            if (Count >= inventorySize) break; // 인벤토리 크기 제한

            int addCount = 0;
            if (item.maxStackCount <= 0)
                addCount = 1;
            else
                addCount = (int)Mathf.Min(item.maxStackCount, remaining);
            var newStack = new T
            {
                code = item.code,
                iconCode = item.iconCode,
                count = addCount,
                maxStackCount = item.maxStackCount
            };
            stacks.Add(newStack);
            remaining -= addCount;
        }

        // 남은 아이템 반환
        if (remaining > 0)
        {
            var remainingItem = new T
            {
                code = item.code,
                iconCode = item.iconCode,
                count = (int)remaining,
                maxStackCount = item.maxStackCount
            };
            return remainingItem;
        }
        else
        {
            return null;
        }
    }

    public virtual void Remove(T item)
    {
        if (!inventoryDict.TryGetValue(item.code, out var stacks)) return;

        stacks.Remove(item);
        if (stacks.Count == 0)
            inventoryDict.Remove(item.code);
    }

    public virtual void SubItem(string code, float count)
    {
        if (!inventoryDict.TryGetValue(code, out var stacks)) return;

        for (int i = 0; i < stacks.Count && count > 0; i++)
        {
            var stack = stacks[i];
            int toRemove = (int)Mathf.Min(stack.count, count);
            stack.count -= toRemove;
            count -= toRemove;

            if (stack.count <= 0f)
            {
                stacks.RemoveAt(i);
                i--;
            }
        }

        if (stacks.Count == 0)
            inventoryDict.Remove(code);
    }

    public int GetTotalCount(string code)
    {
        if (!inventoryDict.TryGetValue(code, out var stacks)) return 0;
        return stacks.Sum(i => i.count);
    }

    public T FindFirst(string code)
    {
        if (!inventoryDict.TryGetValue(code, out var stacks) || stacks.Count == 0)
            return null;

        return stacks.OrderByDescending(item => item.count).First();
    }

    public List<T> FindAll(string code)
    {
        if (!inventoryDict.TryGetValue(code, out var stacks)) return new List<T>();
        return stacks;
    }

    public void Clear()
    {
        inventoryDict.Clear();
    }

    public bool Contains(T item)
    {
        return inventoryDict.TryGetValue(item.code, out var stacks) && stacks.Contains(item);
    }

    public Dictionary<string, List<T>> GetRawInventory() => inventoryDict;
    public List<T> GetInventoryItemToList()
    {
        return inventoryDict.Values.SelectMany(list => list).ToList();
    }
}
