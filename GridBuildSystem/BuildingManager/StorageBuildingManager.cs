using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(menuName = "TileGridBuildSystem/BuildingManager/StorageBuildingManager")]
public class StorageBuildingManager : BuildingManager
{
    public override IEnumerator CoFindingTask()
    {
        base.CoFindingTask();
        yield return null;
        while (true)
        {
            yield return ws;
            Task task = FindTask(task =>
                (task.eJobType == EJOB_TYPE.Delivery || task.eJobType == EJOB_TYPE.Recall) &&
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
        Task task = taskManager.GetTask();
        if (task == null)
            return;
        
    }
    
}
