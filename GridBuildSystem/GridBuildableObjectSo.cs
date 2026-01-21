using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(menuName = "TileGridBuildSystem/GridBuildableSo")]
public class GridBuildableObjectSo : ScriptableObject
{ 
    [Header("BuildingInfo")]
    public string assetName;
    [TextArea(3,10)]
    public string dec;
    public Sprite icon;
    public EBuildingCategory type;
    public Vector3Int size;

    [Header("Setting")]
    public Color ghostObjectColor = new Color(1f, 1f, 1f, 0.5f);
}
