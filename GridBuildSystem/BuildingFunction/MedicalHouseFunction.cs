using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TileGridBuildSystem/BuildingFunction/MedicalHouseFunction")]
public class MedicalHouseFunction : BuildingFunction
{
    MedicalHouseBuildingManager manager => myBuilding.buildingManager as MedicalHouseBuildingManager;

    public override void Init(GridBuilding _gridBuilding, Define.EINTERACTION_BUILDING_TYPE _type)
    {
        base.Init(_gridBuilding, _type);
        myBuilding.StartCoroutine(CoHealingLoop());
    }

    public override void Interaction(InteractiveUnit unit, UnityAction onComplete = null)
    {
        onComplete.Invoke();
        if (manager.patientsList.Count >= manager.bedCount)
            return;

        // 이미 입원 중인 유닛이면 무시
        if (manager.patientsList.Contains(unit))
            return;

        // 유닛 입원 처리
        manager.patientsList.Add(unit);

        UnityPoolManager.Instance.Release(unit.gameObject);

        // 치료 타이머 초기화 (만약 필요시 지정)
        if (unit.medicalProgress.healingTime <= 0f)
        {
            unit.medicalProgress.healingTime = unit.isDeath ? 30f : 5f; // 임시값 또는 레벨 기반 등
            unit.medicalProgress.runTime = 0f;
        }

    }
    private IEnumerator CoHealingLoop()
    {
        while (true)
        {
            if (manager == null)
                yield return null;
            else
            {
                if (manager.patientsList.Count > 0)
                {
                    for (int i = manager.patientsList.Count - 1; i >= 0; i--)
                    {
                        InteractiveUnit unit = manager.patientsList[i];
                        MedicalProgress progress = unit.medicalProgress;

                        progress.runTime += Time.deltaTime;

                        if (progress.runTime >= progress.healingTime)
                        {
                            unit.playStatus.PlusStatus("HP", unit.playStatus.GetStatus("MaxHP"));
                            if (unit.isDeath == true)
                                unit.isDeath = false;

                            progress.runTime = 0f;
                            progress.healingTime = 0f;
                            manager.patientsList.RemoveAt(i);

                            InteractiveUnit respawnUnit = UnityPoolManager.Instance.Spawn<InteractiveUnit>(unit.gameObject, myBuilding.interactionPoint.position);
                            

                            Debug.Log($"[병원] {unit.name} 치료 완료");
                        }
                    }
                }
            }

            yield return null;
        }
    }
}
