using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UIGridBuildingCategory : UIWindowBase
{
    public EBuildingCategory eCurrentTab;
    public UITapGroup group;
    public UIGridBuildableView gridBuildableList;
    BuildableObjectInfoTable buildableObjectInfoTable => GameDataBase.instance.buildableObjectInfoTable;
    List<BuildableObjectInfoTable.Data> BuildableList => buildableObjectInfoTable.FindAll(eCurrentTab);

    /*
     * *
     */

    public override void Open()
    {
        base.Open();
        group?.Set((int)eCurrentTab);
    }
    public void OnChangeTab()
    {
        eCurrentTab = (EBuildingCategory )group._currentTapGroupBtn._index;
        gridBuildableList.Open(BuildableList);
    }
}
