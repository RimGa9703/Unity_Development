using GDBA;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherBuilding : GridBuilding
{
    [ShowInInspector]
    public GatherData gatherData;

    public override void SetData(BuildableObjectInfoTable.Data _data, Color ghostColor)
    {
        base.SetData(_data, ghostColor);
         
        GatherObjectInfoTable.Data data = GameDataBase.instance.gatherObjectInfoTable.Find(Data.code);   
        gatherData = new GatherData();
        gatherData.gatherType = data.eGather_Type;
        gatherData.inventory = new GameItemInventory();

        for (int i = 0; i < data.materials.Length; i++)
        {    
            MoneyItemInfoTable.Data moneyData = GameDataBase.instance.moneyItemInfoTable.Find(data.materials[i].code);
            GameItem gameItem = new GameItem
            {
                code = data.materials[i].code,
                count = data.materials[i].count,
                iconCode = moneyData.iconCode,
                maxStackCount = moneyData.maxStackCount
            };

            gatherData.inventory.AddItem(gameItem);
        }
    }
}
