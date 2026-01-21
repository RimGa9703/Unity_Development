using Only1Games.Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TileGridBuildSystem/BuildingFunction/ConstrutionFunction")]
public class ConstrutionFunction : BuildingFunction
{
    public override void Interaction(InteractiveUnit unit, UnityAction onComplete = null)
    {
        myBuilding.StartCoroutine(DeliverMaterialRoutine(unit, onComplete));
    }

    private IEnumerator DeliverMaterialRoutine(InteractiveUnit unit, UnityAction onComplete)
    {
        float delay = GetDeliverDelay(unit.level);
        List<GameItem> moneyItems  = unit.GetMaterialsFromInventory(unit.task.taskDetail);
        foreach (var item in moneyItems)
        {
            int itemCount = Mathf.FloorToInt(item.count);
            for (int i = 0; i < itemCount; i++)
            {
                DTProjectUtil.SpawnItemEffect(item.iconCode, unit.popupCenter.position, myBuilding.transform.position);
                unit.task.taskDetail.AddRequiredMaterial(item.code, 1);
                yield return new WaitForSeconds(delay);
            }
        }

        onComplete?.Invoke();
    }

    float GetDeliverDelay(int level, float baseDelay = 5f, float delayStep = 1f, float minDelay = 3f)
    {
        float delay = baseDelay - (level - 1) * delayStep;
        return Mathf.Max(delay, minDelay);
    }
}
