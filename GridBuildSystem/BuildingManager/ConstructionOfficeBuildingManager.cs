using GDBA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class ConstructionOfficeBuildingManager : BuildingManager
{
    public override IEnumerator CoFindingTask()
    {
        base.CoFindingTask();
        yield return null;
        while (true)
        {
            yield return ws;
            Task task = FindTask(task =>
                task.eJobType == EJOB_TYPE.Build &&
                task.target == myBuilding.transform);
            if (task != null)
            {
                taskManager.AddTask(task);
                ProcessTasks();
            }
            DispatchNpc();
        }
    }
    public override void ProcessTasks()
    {
        //1. 작업 할 작업 예정 목록이 있는가?
        //2. 일할 NPC가 있는가?
         
        Task task = taskManager.GetTask();
        if (task == null)
        {
            //더 이상 일감이 없고 인벤토리에 불필요한 아이템이 있을 경우 회수
            if (myBuilding.buildingManager.Inventory.Count > 0)
            {
                TaskDetail recallTaskDetall = new TaskDetail();
                foreach (var item in myBuilding.buildingManager.Inventory.GetRawInventory())
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        GameItem item1 = item.Value[i];
                        RequiredMaterial requiredMaterial = new RequiredMaterial();
                        requiredMaterial.code = item1.code;
                        requiredMaterial.iconCode = item1.iconCode;
                        requiredMaterial.requiredMaterialCount = item1.count;
                        requiredMaterial.submittedMaterialCount = 0;
                        recallTaskDetall.requiredMaterials[requiredMaterial.code] = requiredMaterial;
                    }
                }
                
                TaskHub.Instance.GenarateTask(myBuilding.transform, GridBuildSystem.Instance.GetNearestBuilding(myBuilding, STORAGE).transform, recallTaskDetall, EJOB_TYPE.Recall);
            }
            return;
        }

        if (myBuilding.IsInventoryInsufficient(task.taskDetail) == true)
        {
            TaskDetail buildingProgress = myBuilding.buildingManager.Inventory.ApplyMaterialShortages(task.taskDetail);
            TaskHub.Instance.GenarateTask(myBuilding.transform, GridBuildSystem.Instance.GetNearestBuilding(myBuilding, STORAGE).transform, buildingProgress, EJOB_TYPE.Delivery);
        }
    }
}
