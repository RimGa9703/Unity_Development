using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using static Define;


public class LoggingCampBuildingManager : BuildingManager
{
    public override IEnumerator CoFindingTask()
    {
        base.CoFindingTask();
        yield return null;
        while (true)
        {
            yield return ws;
            Task task = FindTask(task =>
                task.eJobType == EJOB_TYPE.Logging &&
                task.owner == myBuilding.transform);

            if (task != null)
            {
                taskManager.AddTask(task);
                ProcessTasks();
            }
            RecallMaterial();
            DispatchNpc();
        }
    } 
    
    public override void DispatchNpc()
    {
        NPCUnit unit = GetCanWorkNpc();
        if (unit == null)
            return;
        if (myBuilding.buildingManager.Inventory.Count >= myBuilding.maxInventorySize)
            return;

        Task task = taskManager.GetTask();
        if (task == null)
        {
            GatherBuilding gather = GridBuildSystem.Instance.GetNearestBuilding(myBuilding, "Tree") as GatherBuilding;
            if (gather == null)
                return;
            TaskDetail taskDetail = new TaskDetail();
            foreach (var item in gather.gatherData.inventory.GetRawInventory())
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    GameItem item1 = item.Value[i];
                    RequiredMaterial requiredMaterial = new RequiredMaterial();
                    requiredMaterial.code = item1.code;
                    requiredMaterial.iconCode = item1.iconCode;
                    requiredMaterial.requiredMaterialCount = 10;
                    requiredMaterial.submittedMaterialCount = 0;
                    taskDetail.requiredMaterials[requiredMaterial.code] = requiredMaterial;
                }
            }

            TaskHub.Instance.GenarateTask(myBuilding.transform, gather.transform, taskDetail, Define.EJOB_TYPE.Logging);
            return;
        }
        task.worker = unit;
        unit.isWorkStart = true;
        unit.task = task;
    }
    public void RecallMaterial()
    {
        GridBuilding storage = GridBuildSystem.Instance.GetNearestBuilding(myBuilding, STORAGE) as FunctionalBuilding;
        if (storage == null)
            return;
        Task task= storage.buildingManager.taskManager.GetTask(EJOB_TYPE.Recall);
        if (task != null)
            return;
        if (myBuilding.buildingManager.Inventory.Count >= myBuilding.maxInventorySize)
        {
            TaskDetail taskDetail = new TaskDetail();

            foreach (var item in myBuilding.buildingManager.Inventory.GetRawInventory())
            {
                GameItem item1 = item.Value[0];
                
                RequiredMaterial requiredMaterial = new RequiredMaterial();
                requiredMaterial.code = item1.code;
                requiredMaterial.iconCode = item1.iconCode;
                requiredMaterial.requiredMaterialCount = myBuilding.buildingManager.Inventory.GetTotalCount(item1.code);
                requiredMaterial.submittedMaterialCount = 0;
                taskDetail.requiredMaterials[requiredMaterial.code] = requiredMaterial;
            }

            TaskHub.Instance.GenarateTask(myBuilding.transform, GridBuildSystem.Instance.GetNearestBuilding(myBuilding, STORAGE).transform, taskDetail, Define.EJOB_TYPE.Recall);
        }
    }
}
