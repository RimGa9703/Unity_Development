using GDBA;
using Only1Games.Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TileGridBuildSystem/BuildingFunction/SubmitMaterialFunction")]
public class SubmitMaterialFunction : BuildingFunction
{
    public override void Interaction(InteractiveUnit unit, UnityAction onComplete = null)
    {
        myBuilding.StartCoroutine(DeliverMaterialRoutine(unit, onComplete));
    }

    private IEnumerator DeliverMaterialRoutine(InteractiveUnit unit, UnityAction onComplete)
    {
        yield return null;
        
         List<GameItem> moneyItems = unit.GetMaterialsFromInventory(unit.task.taskDetail);
        
        foreach (var item in moneyItems)
        {
            int itemCount = Mathf.FloorToInt(item.count);
            DTProjectUtil.SpawnItemEffect(item.iconCode, unit.popupCenter.position, myBuilding?.transform.position ?? unit.popupCenter.position);
            unit.task.taskDetail.AddRequiredMaterial(item.code, itemCount);

            //if ((gridBuilding as FunctionalBuilding).Data.code.Equals(Define.STORAGE) == true)
            //    GameDataBase.instance.playerInfo.storageInventory.AddItem(item);
            //else
                (myBuilding as FunctionalBuilding).buildingManager.Inventory.Add(item);
        }

        

        onComplete?.Invoke();
    }

    float GetDeliverDelay(int level, float baseDelay = 5f, float delayStep = 1f, float minDelay = 3f)
    {
        float delay = baseDelay - (level - 1) * delayStep;
        return Mathf.Max(delay, minDelay);
    }
}
