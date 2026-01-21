using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildSystemUIManager : UIWindowBase
{
    public UIGridBuildingCategory uIGridBuildingCategory;

    /*
     * *
     */

    public void OnClickOpenEditMode()
    {
        if (uIGridBuildingCategory.IsOpen == true)
            uIGridBuildingCategory.Close();
        else
            uIGridBuildingCategory.Open();
    }
}
