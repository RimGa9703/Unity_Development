using GDBA;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "TileGridBuildSystem/BuildingFunction/GatherMaterialFunction")]
public class GatherMaterialFunction : BuildingFunction
{
    public override void Interaction(InteractiveUnit unit, UnityAction onComplete = null)
    {
        myBuilding?.StartCoroutine(CoInteraction(unit,onComplete));
    }
    IEnumerator CoInteraction(InteractiveUnit unit, UnityAction onComplete = null)
    {
        
        yield return new WaitForSeconds(1f);

        GatherData gatherData = (myBuilding as GatherBuilding).gatherData;
        //TODO : 필요한 양만큼 추출한것 여기에서 또 몇개를 획득 할 것인가를 구현해야야함

        if (unit is NPCUnit)
        {
            List<GameItem> itemList = DTProjectUtil.GetMaterialsFromInventory(gatherData.inventory, unit.task.taskDetail);
            GridBuilding targetBuilding = unit.task.target.GetComponent<GridBuilding>();

            foreach (var item in itemList)
            {
                DTProjectUtil.SpawnItemEffect(item.iconCode, targetBuilding.interactionPoint.position, unit.popupCenter.position);
                unit.inventory.Add(item);
            }
            if (gatherData.inventory.Count <= 0)
            {
                myBuilding?.RemovePlace();
                unit.task.target = GridBuildSystem.Instance.GetNearestBuilding(myBuilding, targetBuilding.Data.code).transform;
            }
        }
        else
        {
            List<string> keys = gatherData.inventory.GetRawInventory().Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                GameItem item = gatherData.inventory.FindFirst(keys[i]);
                DTProjectUtil.SpawnItemEffect(item.iconCode, myBuilding.interactionPoint.position, unit.popupCenter.position);
                gatherData.inventory.TransferItem(keys[i],unit.inventory,10);
            } 
        }
        onComplete?.Invoke();
    }
}
