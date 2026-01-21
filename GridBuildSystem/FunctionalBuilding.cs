using GDBA;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionalBuilding : GridBuilding
{
    

    public int maxInventorySize = 5;

    public override void SetData(BuildableObjectInfoTable.Data _data, Color ghostColor)
    {
        base.SetData(_data, ghostColor);
        buildingManager.Inventory.InventorySize = maxInventorySize;
        buildingManager.Inventory.Clear();
        //for (int i = 0; i < GameDataBase.instance.moneyItemInfoTable.Count; i++)
        //{
        //    MoneyItem moneyItem = new MoneyItem();
        //    moneyItem.code = GameDataBase.instance.moneyItemInfoTable.table[i].code;
        //    moneyItem.count = 99999999;
        //    moneyItem.iconCode = GameDataBase.instance.moneyItemInfoTable.table[i].iconCode;
        //    inventory.Add(moneyItem);
        //}
        MoneyItem moneyItem = new MoneyItem();
        moneyItem.code = "wood";
        moneyItem.count = 20;
        moneyItem.iconCode = GameDataBase.instance.moneyItemInfoTable.Find(moneyItem.code).iconCode;
        moneyItem.maxStackCount = GameDataBase.instance.moneyItemInfoTable.Find(moneyItem.code).maxStackCount;
        buildingManager.Inventory.Add(moneyItem);

        moneyItem = new MoneyItem();
        moneyItem.code = "stone";
        moneyItem.count = 20;
        moneyItem.iconCode = GameDataBase.instance.moneyItemInfoTable.Find(moneyItem.code).iconCode;
        moneyItem.maxStackCount = GameDataBase.instance.moneyItemInfoTable.Find(moneyItem.code).maxStackCount;
        buildingManager.Inventory.Add(moneyItem);

        if (buildingManager == null)
            buildingManager = (BuildingManager)ScriptableObject.CreateInstance(_data.managerCode);
    }

    [Button("OnHireNPC", ButtonSizes.Medium), TitleGroup("Building Cheat")]
    public void OnHireNPC()
    {
        NPCManager.Instance.HireNPC(this);
        ExampleActivity.Instance.isNeedRefresh = true;
    }

    #region Inventory
    public GameItem GetItemFromInventory(string code)
    {
       return buildingManager.Inventory.FindItem(code);
    }
    [Button("Add Item", ButtonSizes.Small, ButtonStyle.CompactBox), TitleGroup("Building Cheat")]
    public void AddItemToInventory(string code,int count)
    {
        GameItem item = new GameItem
        {
            code = code,
            count = count,
            iconCode = GameDataBase.instance.moneyItemInfoTable.Find(code).iconCode,
            maxStackCount = GameDataBase.instance.moneyItemInfoTable.Find(code).maxStackCount
        };
        buildingManager.Inventory.Add(item); 
    }

    
    public bool IsInventoryInsufficient(TaskDetail buildingProgress)
    {
        if (buildingProgress == null)
            return false;

        foreach (var item in buildingProgress.requiredMaterials)
        {
            string code = item.Value.code;
            float requiredCount = item.Value.requiredMaterialCount;

            // 해당 코드가 인벤토리에 없으면 true (필요 재료 부족)
            if (buildingManager.Inventory.HasItem(code) == false)
                return true;

            // 인벤토리 총 개수가 부족하면 true
            if (buildingManager.Inventory.GetTotalCount(code) < requiredCount)
                return true;
        }
        return false;
    }
    
    public void SubItem(string code,float count)
    {
        buildingManager.Inventory.SubItem(code, count);
    }
    public void AddItem(GameItem item)
    {
        buildingManager.Inventory.AddItem(item);
    }
    #endregion

}

