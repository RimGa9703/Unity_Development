using GDBA;
using Only1Games.Assets;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;



public class GridBuildSystem : MonoSingleton<GridBuildSystem>
{
    private static Dictionary<TileType, TileBase> dicTile = new();
    [SerializeField] private SerializableDictionary<string, List<GridBuildIngData>> dicPlaced = new();

    public static int buildingUID = 0;

    public GridBuildSystemUIManager gridBuildSystemUIManager;
    public GridLayout gridLayout;
    public Tilemap mainTileMap;
    public Tilemap tempTilemap;
    public Tilemap placeTilemap;
    public Camera cam;
    public bool isSelectedBuilding = false;


    public enum TileType
    {
        Empty,
        White,
        Green,
        Red,
    }

    private BoundsInt prevArea;
    BuildableObjectInfoTable buildableObjectInfoTable => GameDataBase.instance.buildableObjectInfoTable;

    /*
     * *
     */
    #region UnityFunc
    public void Init()
    {
        dicTile.Clear();
        dicTile.Add(TileType.Empty, null);
        dicTile.Add(TileType.White, Resources.Load<TileBase>(AssetPath.Tile + "white"));
        dicTile.Add(TileType.Green, Resources.Load<TileBase>(AssetPath.Tile + "green"));
        dicTile.Add(TileType.Red, Resources.Load<TileBase>(AssetPath.Tile + "red"));

        buildingUID = 0;

        SpawnResourcesByTileName("Tree", buildableObjectInfoTable.Find("Tree"));
        SpawnResourcesByTileName("Stone", buildableObjectInfoTable.Find("Panel"));
        BakeTilemapToWalkable();
    }

    #endregion

    #region BuildingPlacement
    public void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        tempTilemap.SetTilesBlock(prevArea, toClear);
    }
    public void ClearArea(Tilemap tilemap, BoundsInt area)
    {
        TileBase[] toClear = new TileBase[area.size.x * area.size.y * area.size.z];
        FillTiles(toClear, TileType.Empty);
        tilemap.SetTilesBlock(area, toClear);
    }
   
    public void UpdatePlacementPreview(GridBuilding gridBuilding)
    {
        ClearArea();
        gridBuilding.area.position = gridLayout.WorldToCell(gridBuilding.transform.position);
        BoundsInt buildingArea = gridBuilding.area;

        TileBase[] baseArray = GetTilesBlock(buildingArea, mainTileMap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == dicTile[TileType.White])
                tileArray[i] = dicTile[TileType.Green];
        }

        if (CanTakeArea(buildingArea) == false)
        {
            FillTiles(tileArray, TileType.Red);
        }

        tempTilemap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }
    // 해당 구역에 설치 가능한가 여부 체크
    public bool CanTakeArea(BoundsInt area)
    {
        GridBuildIngData data = GetPlacedDataOnTiles(area);
        if (data == null || data.gridBuilding.placed == false)
            return true;

        return false;
    }
    // 해당 구역에 영역 건물이 점령하게 하는 함수
    public void TakeArea(GridBuildIngData gridBuildIngData)
    {
        SetTilesBlock(gridBuildIngData.placedArea, TileType.Empty, tempTilemap);
        SetTilesBlock(gridBuildIngData.placedArea, TileType.White, placeTilemap);

        if (!dicPlaced.TryGetValue(gridBuildIngData.code, out var list))
        {
            list = new List<GridBuildIngData>();
            dicPlaced[gridBuildIngData.code] = list;
        }

        list.Add(gridBuildIngData);

        StartCoroutine(CoDelayedScan());
    }
    IEnumerator CoDelayedScan()
    {
        yield return null;
        AstarPath.active.Scan();
    }

    public void RemoveGridBuilding(BoundsInt area, int UID)
    {
        foreach (var kvp in dicPlaced)
        {
            var list = kvp.Value;
            var target = list.FirstOrDefault(data => data.gridBuilding.UID == UID);
            if (target != null)
            {
                SetTilesBlock(target.placedArea, TileType.Empty, placeTilemap);
                SetTilesBlock(target.placedArea, TileType.White, mainTileMap);
                list.Remove(target);
                if (list.Count == 0)
                    dicPlaced.Remove(kvp.Key);
                break;
            }
        }
        StartCoroutine(CoDelayedScan());
    }
    #endregion

    #region TileSystem
    public void BakeTilemapToWalkable()
    {
        GridGraph grid = AstarPath.active.data.gridGraph;
        BoundsInt bounds = mainTileMap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase tile = mainTileMap.GetTile(cellPos);

                if (tile == null)
                {
                    Vector3 worldPos = mainTileMap.CellToWorld(cellPos) + mainTileMap.cellSize / 2f;
                    Bounds nodeBounds = new Bounds(worldPos, Vector3.one * 0.1f);

                    GraphUpdateObject guo = new GraphUpdateObject(nodeBounds);
                    guo.modifyWalkability = false;
                    guo.setWalkability = false;

                    AstarPath.active.UpdateGraphs(guo);
                }
            }
        }
    }

    public static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int count = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[count] = tilemap.GetTile(pos);
            count++;
        }
        return array;
    }
    public GridBuildIngData GetPlacedDataOnTiles(BoundsInt area)
    {
        foreach (var list in dicPlaced.Values)
        {
            foreach (var data in list)
            {
                if (IsOverlap(data.placedArea, area))
                    return data;
            }
        }
        return null;
    }

    private bool IsOverlap(BoundsInt a, BoundsInt b)
    {
        // x축 겹침
        bool xOverlap = a.xMin < b.xMax && a.xMax > b.xMin;
        // y축 겹침
        bool yOverlap = a.yMin < b.yMax && a.yMax > b.yMin;

        return xOverlap && yOverlap;
    }

    public static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }
    static void FillTiles(TileBase[] tileArray, TileType type)
    {
        for (int i = 0; i < tileArray.Length; i++)
        {
            tileArray[i] = dicTile[type];
        }
    }

    public void SpawnResourcesByTileName(string tileName, BuildableObjectInfoTable.Data resourceData)
    {
        BoundsInt bounds = placeTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            Vector3Int cellPos = new Vector3Int(pos.x, pos.y, 0);
            TileBase tile = placeTilemap.GetTile(cellPos);

            if (tile != null && tile.name == tileName)
            {
                // 1. 자원 설치
                SpawnEnvironmentObject(resourceData, cellPos);

                // 2. 해당 타일을 white 타일로 교체
                placeTilemap.SetTile(cellPos, dicTile[TileType.White]);
            }
        }
        StartCoroutine(CoDelayedScan());
    }

    public void SpawnEnvironmentObject(BuildableObjectInfoTable.Data data, Vector3Int cellPosition)
    {
        GameObject prefab = AssetManager.Instance.GetAsset<GameObject>(AssetPath.Building, data.code);
        GridBuilding building = UnityPoolManager.Instance.Spawn<GridBuilding>(prefab, Vector3.zero);

        building.SetData(data, Define.ghostObjectColor);
        building.EditMode = false;
        building.placed = true;

        Vector3 worldPos = gridLayout.CellToLocalInterpolated(cellPosition + new Vector3(0.5f, 0.5f, 0f));
        building.transform.localPosition = worldPos;

        BoundsInt area = new BoundsInt(cellPosition, new Vector3Int(data.width, data.height, 1));

        GridBuildIngData buildData = new GridBuildIngData()
        {
            code = data.code,
            level = 1,
            gridBuilding = building,
            placedArea = area
        };

        building.UID = ++buildingUID;

        if (!dicPlaced.TryGetValue(data.code, out var list))
        {
            list = new List<GridBuildIngData>();
            dicPlaced[data.code] = list;
        }
        list.Add(buildData);
        SetTilesBlock(area, TileType.White, placeTilemap);
    }

    public GridBuilding GetNearestBuilding(GridBuilding fromBuilding, string code)
    {
        if (!dicPlaced.TryGetValue(code, out var buildingList) || buildingList.Count == 0)
            return null;

        GridBuilding closest = null;
        float minDistance = float.MaxValue;
        Vector3 fromPos = fromBuilding.transform.position;

        foreach (var storage in buildingList)
        {
            if (storage.gridBuilding == null) continue;

            float dist = Vector3.Distance(fromPos, storage.gridBuilding.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = storage.gridBuilding;
            }
        }

        return closest;
    }
    public GridBuilding GetNearestBuilding(Vector3 myPosition,string code)
    {
        if (!dicPlaced.TryGetValue(code, out var buildingList) || buildingList.Count == 0)
            return null;

        GridBuilding closest = null;
        float minDistance = float.MaxValue;
        Vector3 fromPos = myPosition;

        foreach (var storage in buildingList)
        {
            if (storage.gridBuilding == null) continue;

            float dist = Vector3.Distance(fromPos, storage.gridBuilding.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = storage.gridBuilding;
            }
        }

        return closest;
    }
    public List<GridBuilding> GetNearestBuildings(string code, Vector3 myPosition, int count = 3)
    {
        if (!dicPlaced.TryGetValue(code, out var buildingDataList) || buildingDataList.Count == 0)
            return null;


        var validBuildings = buildingDataList
            .Where(data => data.gridBuilding != null)
            .Select(data => data.gridBuilding)
            .OrderBy(building => Vector3.Distance(myPosition, building.transform.position))
            .Take(count)
            .ToList();

        return validBuildings;
    }
    #endregion

    //////////////
    //////////////////////////////////////////////////

    public void CreateBuilding(BuildableObjectInfoTable.Data data)
    {
        if (isSelectedBuilding == true)
            return;
        isSelectedBuilding = true;
        GameObject copy = AssetManager.Instance.GetAsset<GameObject>(AssetPath.Building, data.code);
        GridBuilding clone = UnityPoolManager.Instance.Spawn(copy, new Vector3(0.5f, 0.5f, 0f)).GetComponent<GridBuilding>();
        clone.SetData(data, Define.ghostObjectColor);
        clone.OpenbuildingEditMode();

        Vector3Int cellPos = gridLayout.LocalToCell(new Vector3(0.5f, 0.5f, 0f));
        clone.transform.position = gridLayout.CellToLocalInterpolated(cellPos + new Vector3(0.5f, 0.5f, 0f));

        UpdatePlacementPreview(clone);
    }

    public GridBuildIngData GetGridBuildIngData(int uid)
    {
        foreach (var list in dicPlaced.Values)
        {
            var data = list.FirstOrDefault(x => x.gridBuilding.UID == uid);
            if (data != null) return data;
        }
        return null;
    }
}
