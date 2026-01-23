using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameItemInventory : StackableInventory<GameItem>
{
    public List<GameItem> FindItemAll(params string[] codes)
    {
        return codes
            .Where(code => inventoryDict.ContainsKey(code))
            .SelectMany(code => inventoryDict[code])
            .ToList();
    }

    public GameItem FindItem(string code) => FindFirst(code);

    public bool IsEnoughItem(string code, float count) => GetTotalCount(code) >= count;

    public void AddItem(string code, int count)
    {
        var existing = FindFirst(code);
        int maxStack = existing?.maxStackCount ?? 99; // 기본값 설정
        Add(new GameItem
        {
            code = code,
            count = count,
            iconCode = existing?.iconCode ?? "",
            maxStackCount = maxStack
        });
    }

    public GameItem AddItem(GameItem item) => Add(item);

    public override void SubItem(string code, float count)
    {
        base.SubItem(code, count); // 기본 로직 호출도 가능
    }

    public float GetItemCount(string code) => GetTotalCount(code);
    public bool HasItem(string code)
    {
        return GetTotalCount(code) > 0f;
    }
    
    public bool CanAddItem(GameItem item)
    {
        // 1. 기존 스택에 충분한 공간이 있는지 확인
        if (inventoryDict.TryGetValue(item.code, out var stacks))
        {
            int spaceInExistingStacks = stacks.Sum(s => s.maxStackCount - (int)s.count);
            if (spaceInExistingStacks >= item.count)
            {
                return true;
            }

            // 2. 새로운 스택을 추가해야 하는 경우, 인벤토리 크기 제한을 확인
            int remaining = (int)item.count - spaceInExistingStacks;
            if (remaining > 0)
            {
                int maxStack = stacks.FirstOrDefault()?.maxStackCount ?? 99; // 기본값 설정
                int newStacksNeeded = (int)Mathf.Ceil((float)remaining / maxStack);
                if (Count + newStacksNeeded <= InventorySize)
                {
                    return true;
                }
            }
        }
        else
        {
            // 3. 아이템 코드가 인벤토리에 없는 경우, 새로운 스택을 추가할 수 있는지 확인
            int maxStack = item.maxStackCount > 0 ? item.maxStackCount : 1;
            int newStacksNeeded = (int)Mathf.Ceil((float)item.count / maxStack);
            if (Count + newStacksNeeded <= InventorySize)
            {
                return true;
            }
        }

        return false;
    }

    // 특정 아이템을 특정 개수만큼 꺼내는 함수
    public List<GameItem> TakeItem(string code, int count)
    {
        List<GameItem> takenItems = new List<GameItem>();

        if (!IsEnoughItem(code, count))
        {
            Debug.LogWarning($"Not enough items with code: {code}. Required: {count}, Available: {GetTotalCount(code)}");
            return takenItems;
        }

        var existingStacks = FindAll(code);
        if (existingStacks.Count == 0) return takenItems; // 아이템이 없는 경우

        // 아이템 복사 후 제거
        int remainingToTake = count;
        foreach (var stack in existingStacks.OrderByDescending(s => s.count))
        {
            if (remainingToTake <= 0) break;

            int takeCount = Mathf.Min(remainingToTake, (int)stack.count);
            takenItems.Add(new GameItem
            {
                code = stack.code,
                count = takeCount,
                iconCode = stack.iconCode,
                maxStackCount = stack.maxStackCount
            });
            remainingToTake -= takeCount;
        }

         SubItem(code, count);

        return takenItems;
    }

    public int TransferItem(string itemCode, GameItemInventory destinationInventory, int maxCount = -1)
    {
        if (!HasItem(itemCode))
        {
            Debug.LogWarning($"Current inventory does not have item with code: {itemCode}");
            return 0;
        }

        // 옮길 개수를 결정합니다.
        int countToTake = (maxCount == -1) ? GetTotalCount(itemCode) : Mathf.Min(GetTotalCount(itemCode), maxCount);

        if (countToTake <= 0)
        {
            return 0;
        }

        // 아이템을 꺼냅니다. (실제 제거는 하지 않음)
        List<GameItem> takenItems = TakeItem(itemCode, countToTake);

        int successfullyAddedCount = 0;

        // 꺼낸 아이템들을 목적지 인벤토리에 추가합니다.
        foreach (var itemStack in takenItems)
        {
            GameItem remainingItem = destinationInventory.AddItem(itemStack);

            // 추가하고 남은 아이템이 있다면
            if (remainingItem != null)
            {
                // 성공적으로 추가된 개수만 계산
                successfullyAddedCount += (int)(itemStack.count - remainingItem.count);
                // 더 이상 추가할 수 없으므로 루프를 종료합니다.
                break;
            }
            else
            {
                // 스택 전체가 성공적으로 추가된 경우
                successfullyAddedCount += (int)itemStack.count;
            }
        }

        // 성공적으로 옮겨진 개수만큼 현재 인벤토리에서 아이템을 차감합니다.
        if (successfullyAddedCount > 0)
        {
            SubItem(itemCode, successfullyAddedCount);
        }

        return successfullyAddedCount;
    }
}