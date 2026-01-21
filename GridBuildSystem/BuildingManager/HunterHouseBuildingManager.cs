using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using static Define;

[CreateAssetMenu(menuName = "TileGridBuildSystem/BuildingManager/HunterHouseBuildingManager")]
public class HunterHouseBuildingManager : BuildingManager
{
    public override IEnumerator CoFindingTask()
    {
        base.CoFindingTask();
        yield return null;
        while (true)
        {
            yield return ws;
            Task task = FindTask(task =>
                (task.eJobType == EJOB_TYPE.Huntting) &&
                task.owner == myBuilding.transform);
            if (task != null)
                taskManager.AddTask(task);   
            else
                ProcessTasks();
            DispatchNpc();
        }
    }
    public override void ProcessTasks()
    {
        //1. 업무가 없고 일할 npc가 있는가? => 업무 생성
        Task task = taskManager.GetTask();
        if (task == null && GetCanWorkNpc() != null)
        {
            FieldArea fieldArea = FieldManager.Instance.GetSpawnerWithLeastPlayerUnits();
            TaskDetail taskDetail = new TaskDetail();
            GameItem item1 = GameItem.MakeItem("meat",10);
            RequiredMaterial requiredMaterial = new RequiredMaterial();
            requiredMaterial.code = item1.code;
            requiredMaterial.iconCode = item1.iconCode;
            requiredMaterial.requiredMaterialCount = item1.count;
            requiredMaterial.submittedMaterialCount = 0;

            taskDetail.requiredMaterials[requiredMaterial.code] = requiredMaterial;

            TaskHub.Instance.GenarateTask(myBuilding.transform, fieldArea.transform, taskDetail, EJOB_TYPE.Huntting);

            return;
        }
    }

    [Button("OnRecruitNPC", ButtonSizes.Medium), TitleGroup("BuildingManager Cheat")]
    public void OnRecruitNPC()
    {
        NPCUnit unit = hireNpcList[0];
        hireNpcList.Remove(unit);
        NPCManager.Instance.NPCUnits.Add(unit);
    }

}
