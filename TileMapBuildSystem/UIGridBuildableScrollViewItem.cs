using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridBuildableScrollViewItem : BaseCellView<GridBuildableItem>
{
    public Image icon;

    public override void SetItem(GridBuildableItem gridBuildableItem)
    {
        base.SetItem(gridBuildableItem);

        //icon.sprite = GameDataBase.instance.buildadbleObjectAtals.GetSprite(gridBuildableItem.data.uiIconCode);
    }
    
}
