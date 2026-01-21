using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TileGridBuildSystem/BuildingManager/TownHallBuildingManager")]
public class TownHallBuildingManager : BuildingManager, ICustomUpdateMono
{
    bool isRefresh = false;
    public float refreshCoolTime = 30f;
    float runCoolTime = 0f;
    
    /*
     * *
     */
    public override void Init(FunctionalBuilding _functionalBuilding)
    {
        base.Init(_functionalBuilding);
        CustomUpdateManager.customUpdateMonos.Add(this);
    }

    public void CustomUpdate()
    {
        runCoolTime += Time.deltaTime;
        if (runCoolTime >= refreshCoolTime)
        {
            runCoolTime = 0;

            if(hireNpcList.Count > 0)
            {
                for (int i = 0; i < hireNpcList.Count; i++)
                {
                    hireNpcList[i].aiPath.SetPath(null);
                    UnityPoolManager.Instance.Release(hireNpcList[i].gameObject);
                }
                hireNpcList.Clear();
            }

            for (int i = 0; i < 3; i++)
            {
                NPCUnit unit = DTProjectUtil.SpawnNPC(myBuilding.transform);
                unit.ownerBuilding = myBuilding;
                hireNpcList.Add(unit);
            }
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
