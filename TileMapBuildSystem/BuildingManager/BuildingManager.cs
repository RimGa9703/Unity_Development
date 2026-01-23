using Only1Games.Assets;
using Only1Games.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BuildingManager : ScriptableObject
{
    [ShowInInspector]
    public TaskManager taskManager;
    [ShowInInspector, ReadOnly]
    public FunctionalBuilding myBuilding;
    [ShowInInspector]
    public List<NPCUnit> hireNpcList = new List<NPCUnit>();

    [SerializeField] private GameItemInventory inventory = new GameItemInventory();
    public GameItemInventory Inventory { get { return inventory; } }

    protected WaitForSeconds ws = new WaitForSeconds(0.2f);
    /*
     * *
     */
    public virtual void Init(FunctionalBuilding _functionalBuilding) 
    {
        if (taskManager == null)
        {
            taskManager = new TaskManager();
            taskManager.Init();
        }
        myBuilding = _functionalBuilding;
        myBuilding.StartCoroutine(CoFindingTask());
    }
    public virtual IEnumerator CoFindingTask()
    {
        yield return null;
    }
    public virtual void ProcessTasks() { }

    public virtual void DispatchNpc() 
    {
        NPCUnit unit = GetCanWorkNpc();
        if (unit == null)
            return;
        
        Task task = taskManager.GetTask();
        if (task == null)
            return;
        task.worker = unit;
        unit.isWorkStart = true;
        unit.task = task;
    }
    [Button("OpenBuildingUI", ButtonSizes.Medium), TitleGroup("BuildingManager UI")]
    public void OnClickOpenBuildingUI()
    {
        UIBuildingInfoWindow UIBuildingInfoWindow = UIWindow.GetWindow<UIBuildingInfoWindow>(AssetPath.UI_WINDOW, "UIBuildingInfoWindow", ExampleActivity.Instance.UIRoot);
        UIBuildingInfoWindow.Init();
        UIBuildingInfoWindow.Open(myBuilding.UID);
    }


    protected Task FindTask(Func<Task, bool> predicate)
    {
        return TaskHub.Instance.GetAvailableTask(predicate);
    }
    public NPCUnit GetCanWorkNpc()
    {
        return hireNpcList.Find(a => a.isWorkStart == false && a.task == null);
    }
    public NPCUnit GetCanWorkNpc(Func<NPCUnit, bool> predicate)
    {
        return hireNpcList.Find(new Predicate<NPCUnit>(predicate));
    }
}
