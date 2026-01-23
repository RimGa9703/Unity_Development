using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(menuName = "TileGridBuildSystem/GridBuildableCategorySo")]
public class GridBuildableCategory : ScriptableObject
{
    [Header("CategoryInfo")]
    public string categotyName;
    public Sprite icon;
    public EBuildingCategory type;
    public List<GridBuildableObjectSo> gridBuildableObjectList = new List<GridBuildableObjectSo>();
}
