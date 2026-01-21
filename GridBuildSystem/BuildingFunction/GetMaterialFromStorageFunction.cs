using GDBA;
using Only1Games.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TileGridBuildSystem/BuildingFunction/GetMaterialFromStorageFunction")]
public class GetMaterialFromStorageFunction : BuildingFunction
{
    public override void Interaction(InteractiveUnit unit, UnityAction onComplete = null)
    {
        myBuilding.StartCoroutine(CoGetMaterialRoutine(unit, onComplete));
    }
    private IEnumerator CoGetMaterialRoutine(InteractiveUnit unit, UnityAction onComplete)
    {
        FunctionalBuilding functionalBuilding = myBuilding as FunctionalBuilding;
        foreach (var item in unit.task.taskDetail.requiredMaterials.Values)
        {
            var requiredMaterial = item;
            var materialCode = requiredMaterial.code;
            var requiredCount = requiredMaterial.requiredMaterialCount;

            var existingItem = unit.inventory.FindItem(item.code);
            float currentCount = existingItem != null ? existingItem.count : 0f;

            float neededCount = requiredCount - currentCount;
            if (neededCount <= 0f)
                continue;

            GameItem gameItem = GameDataBase.instance.playerInfo.storageInventory.FindItem(materialCode);
            int totalCount = GameDataBase.instance.playerInfo.storageInventory.GetTotalCount(materialCode);
            if (totalCount <= 0)
                continue;

            int transferableCount = (int)Mathf.Min(totalCount, neededCount);
            DTProjectUtil.SpawnItemEffect(gameItem.iconCode, functionalBuilding.interactionPoint.position, unit.transform.position);
            GameDataBase.instance.playerInfo.storageInventory.SubItem(materialCode, transferableCount);

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
}
