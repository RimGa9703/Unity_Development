using Only1Games.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGridBuildableView : UIWindowBase
{
    public UIGridBuildableScollView scrollView;

    List<GridBuildableItem> items = new List<GridBuildableItem>();

    /*
     * *
     */

    public void Open(List<BuildableObjectInfoTable.Data> gridBuildableCategory)
    {
        base.Open();

        scrollView.Clear();
        items.Clear();

        for (int i = 0; i < gridBuildableCategory.Count; i++)
        {
            GridBuildableItem gridBuildableItem = new GridBuildableItem();
            gridBuildableItem.data = gridBuildableCategory[i];
            items.Add(gridBuildableItem);
        }
        UIGridBuildableScrollViewItem prefab = AssetManager.Instance.GetAsset<UIGridBuildableScrollViewItem>(AssetPath.UI_SCROLLITEM, "UIGridBuildableScrollViewItem");

        scrollView.SetData(items, prefab, OnClickGridBuildableItem);
    }
    public void OnClickGridBuildableItem(UIGridBuildableScrollViewItem _item)
    {
        GridBuildSystem.Instance.CreateBuilding(_item.Item.data);
    }
}
