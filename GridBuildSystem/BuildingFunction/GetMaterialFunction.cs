using GDBA;
using NUnit.Framework.Constraints;
using Only1Games.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TileGridBuildSystem/BuildingFunction/GetMaterialFunction")]
public class GetMaterialFunction : BuildingFunction
{
    private IEnumerator CoGetMaterialRoutine(InteractiveUnit unit, UnityAction onComplete)
    {
        FunctionalBuilding functionalBuilding = myBuilding as FunctionalBuilding;
        int maxSlotCount = unit.maxInventorySize; // 예: 최대 슬롯 수

        foreach (var item in unit.task.taskDetail.requiredMaterials.Values)
        {
            var requiredMaterial = item;
            var materialCode = requiredMaterial.code;
            var requiredCount = requiredMaterial.requiredMaterialCount;

            var existingItem = unit.inventory.FindItem(materialCode);
            float currentCount = existingItem != null ? existingItem.count : 0;

            float neededCount = requiredCount - currentCount;
            if (neededCount <= 0)
                continue;

            // 현재 건물 인벤토리에서 수량 확인
            int availableFromBuilding = functionalBuilding.buildingManager.Inventory.GetTotalCount(materialCode);
            if (availableFromBuilding <= 0)
                continue;

            // 인벤토리 남은 슬롯 계산
            int usedSlots = unit.inventory.Count;
            int remainingSlots = maxSlotCount - usedSlots;

            // 시뮬레이션: 인벤토리가 이 아이템을 몇 슬롯 더 차지할지 계산
            int additionalSlotsNeeded = 0;
            float remainingToAdd = neededCount;

            // 기존 스택에 남은 공간 사용
            var existingStacks = unit.inventory.FindAll(materialCode);
            foreach (var stack in existingStacks)
            {
                int space = stack.maxStackCount - stack.count;
                float addable = Mathf.Min(space, remainingToAdd);
                remainingToAdd -= addable;
                if (remainingToAdd <= 0) break;
            }

            // 남은 수량은 새 슬롯 필요
            if (remainingToAdd > 0)
            {
                int maxStack = GameDataBase.instance.moneyItemInfoTable.Find(item.code).maxStackCount;
                additionalSlotsNeeded = Mathf.CeilToInt(remainingToAdd / maxStack);
            }

            // 슬롯 초과하면 더 못 넣음
            if (usedSlots + additionalSlotsNeeded > maxSlotCount)
            {
                int allowedNewSlots = maxSlotCount - usedSlots;
                int maxAddableCount = 0;

                // 기존 스택 공간 계산
                foreach (var stack in existingStacks)
                {
                    int space = stack.maxStackCount - stack.count;
                    maxAddableCount += space;
                }

                // 새 슬롯으로 가능한 수량 계산
                int maxStack = GameDataBase.instance.moneyItemInfoTable.Find(item.code).maxStackCount;
                maxAddableCount += allowedNewSlots * maxStack;

                neededCount = Mathf.Min(neededCount, maxAddableCount);
            }

            int transferableCount = Mathf.FloorToInt(Mathf.Min(availableFromBuilding, neededCount));
            if (transferableCount <= 0)
                continue;

            // 아이템 전달
            GameItem gameItem = functionalBuilding.GetItemFromInventory(materialCode);
            SpawnItemEffect(gameItem.iconCode, functionalBuilding.interactionPoint.position, unit.transform.position);
            functionalBuilding.SubItem(materialCode, transferableCount);

            if (currentCount == 0f)
            {
                unit.inventory.Add(gameItem.CloneWithCount(transferableCount));
            }
            else
            {
                unit.inventory.FindItem(materialCode).count += transferableCount;
            }
        }
        yield return new WaitForSeconds(1f);
        onComplete?.Invoke();
    }
    private void SpawnItemEffect(string code, Vector3 startPos, Vector3 endPos)
    {
        var prefab = AssetManager.Instance.GetAsset<GameObject>(AssetPath.FieldObeject, "ItemGivingEffect");
        var item = UnityPoolManager.Instance.Spawn(
            prefab,
            startPos,
            Quaternion.identity,
            new Vector3(0.5f, 0.5f, 0.5f)
        );

        if (item.TryGetComponent(out ItemGivingEffect effect))
        {
            effect.Init(code, startPos, endPos);
        }
    }

    public override void Interaction(InteractiveUnit unit, UnityAction onComplete = null)
    {
        myBuilding.StartCoroutine(CoGetMaterialRoutine(unit, onComplete));
    }
}
