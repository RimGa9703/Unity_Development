using Only1Games.Assets;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Util;
using static Define;


public class GridBuilding : MonoBehaviour, IPointerDownHandler
{
    public int UID;

    public SpriteRenderer sprite;
    public Image buildingProgressGauge;
    public UIBuildingEditMode buildingEditModeUI;
    public Transform interactionPoint;

    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public BuildingManager buildingManager;
    public TaskDetail buildingProgress;

    public float interactionRange = 1f;
     
    #region state
    bool editMode = false;
    public bool EditMode
    {
        get
        {
            return editMode;
        }
        set
        {
            editMode = value;
            if (editMode == true)
            {
                GridBuildSystem.Instance.isSelectedBuilding = true;
                JoystickController.Instance.UIJoystick.IsCanTouch = false;
                sprite.color = ghostColor;
            }
            else
            {
                GridBuildSystem.Instance.isSelectedBuilding = false;
                JoystickController.Instance.UIJoystick.IsCanTouch = true;
                if (buildingProgress?.CheckRequiredMaterial() == false)
                    return;
                sprite.color = origine;
            }
        }
    }

    public bool placed = false;
    public EBUILDING_STATE eCurrentState;
    #endregion

    public BoundsInt area;
    private Color origine = new Color(1f, 1f, 1f, 1f);
    private Color ghostColor = new Color(1f, 1f, 1f, 1f);
    private Vector3 prevPos;

    private SerializableDictionary<EINTERACTION_BUILDING_TYPE, BuildingFunction> buildingFunctionDic = new SerializableDictionary<EINTERACTION_BUILDING_TYPE, BuildingFunction>();

    #region Get/set
    private BuildableObjectInfoTable.Data data = null;
    public BuildableObjectInfoTable.Data Data
    {
        get { return data; }
        set { data = value; }
    }
    #endregion

    /*
     * *
     */

    public virtual void SetData(BuildableObjectInfoTable.Data _data, Color ghostColor)
    {
        data = _data;
        EditMode = true;
        eCurrentState = EBUILDING_STATE.Idle;

        area.size = new Vector3Int(data.width, data.height, 1);
        this.ghostColor = ghostColor;
        sprite.color = ghostColor;

        for (int i = 0; i < _data.buildingFunctions.Length; i++)
        {
            BuildingFunction buildingFunction = (BuildingFunction)ScriptableObject.CreateInstance(_data.buildingFunctions[i].code);
            buildingFunctionDic[_data.buildingFunctions[i].type] = buildingFunction;
            buildingFunctionDic[_data.buildingFunctions[i].type].Init(this, _data.buildingFunctions[i].type);
        }

        buildingProgressGauge?.gameObject.SetActive(false);

        if (buildingProgress == null)
            buildingProgress = new TaskDetail();
        buildingProgress.SetData(data.requiredMaterials);

        //StartCoroutine(CoCheckInteractionArea());

    }
    public void Update()
    {
        RefreshBuildingProgress();
        if (EditMode == false)
            return;

        if (Input.GetMouseButton(0) == true)
        {
            if (EventSystem.current.IsPointerOverGameObject(0))
                return;

            if (placed == false)
            {
                Vector2 mousePos = GridBuildSystem.Instance.cam.ScreenToWorldPoint(Input.mousePosition);
                RefreshBuildingPlacementPreview(mousePos);
            }
        }
    }
    //건물 설치전 청사진 Preview하기 위한 함수
    public void RefreshBuildingPlacementPreview(Vector3 pos)
    {
        if (placed == false)
        {
            Vector2 mousePos = pos;
            Vector3Int cellPos = GridBuildSystem.Instance.gridLayout.LocalToCell(mousePos);

            if (prevPos != cellPos)
            {
                BoundsInt buildingArea = area;
                buildingArea.position = cellPos;
                TileBase[] baseArray = GridBuildSystem.GetTilesBlock(buildingArea, GridBuildSystem.Instance.mainTileMap);
                bool hasTileNull = baseArray.Any(tile => tile == null);

                prevPos = cellPos;
                if (hasTileNull == false)
                {
                    transform.localPosition = GridBuildSystem.Instance.gridLayout.CellToLocalInterpolated(cellPos + new Vector3(0.5f, 0.5f, 0f));
                    GridBuildSystem.Instance.UpdatePlacementPreview(this);
                }
            }
        }
    }

    //건설이 진행중일 때 진행도를 UI에 표시
    void RefreshBuildingProgress()
    {
        if (buildingProgress == null)
            return;
        if (buildingProgressGauge != null)
        {
            buildingProgressGauge?.gameObject.SetActive(true);
            buildingProgressGauge.fillAmount = buildingProgress.GetProgressPercent();
        }

        if (buildingProgress.CheckRequiredMaterial() == true)
        {
            sprite.color = origine;
            buildingProgress.Clear();
            buildingProgress = null;

            buildingProgressGauge?.gameObject.SetActive(false);

            FunctionalBuilding functionalBuilding = this as FunctionalBuilding;
            if (functionalBuilding != null)
            {
                if (functionalBuilding.buildingManager != null)
                    functionalBuilding.buildingManager.Init(functionalBuilding);
            }
        }
    }
    //현재 npc가 건물과 상호작용 할 수 있는가 판단.
    public bool CanNpcInteract(BaseUnit unit)
    {
        if (unit == null) return false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(interactionPoint.position, interactionRange, LayerMask.NameToLayer(Layers.Player));
        return colliders.Any(c => c == unit.myCollider);
    }
    //해당 지역에 건물을 설치 할 수 있는지 판단
    public bool CanBePlaced()
    {
        Vector3Int positionInt = GridBuildSystem.Instance.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;

        if (GridBuildSystem.Instance.CanTakeArea(areaTemp) == true)
            return true;
        return false;
    }
    //건물 설치
    public void Place()
    {
        if (CanBePlaced() == false)
            return;

        Vector3Int positionInt = GridBuildSystem.Instance.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;
        placed = true;
        EditMode = false;
        //sprite.color = origine;
        GridBuildIngData gridBuildIngData = GridBuildSystem.Instance.GetGridBuildIngData(UID);
        if (gridBuildIngData != null)
        {
            GridBuildSystem.Instance.ClearArea(GridBuildSystem.Instance.placeTilemap, gridBuildIngData.placedArea); 
            gridBuildIngData.gridBuilding = this;
            gridBuildIngData.placedArea = areaTemp;
        }
        else
        {
            gridBuildIngData = new GridBuildIngData()
            {
                code = data.code,
                level = 1,
                gridBuilding = this,
                placedArea = areaTemp
            };

            GridBuildSystem.buildingUID++;
            UID = GridBuildSystem.buildingUID;
            eCurrentState = EBUILDING_STATE.Constrution;

            TaskHub.Instance.GenarateTask(transform, GridBuildSystem.Instance.GetNearestBuilding(this, CONSTRUCTION_OFFICE)?.transform, buildingProgress, EJOB_TYPE.Build);
        }

        Bounds bounds = new Bounds();
        bounds.SetMinMax(gridBuildIngData.placedArea.min, gridBuildIngData.placedArea.max);

        GridBuildSystem.Instance.TakeArea(gridBuildIngData);

        buildingEditModeUI.Close();
    }
    //건물을 옮길 때 취소하면 이전에 설치되어있던 장소로 되돌아가기 위한 함수
    public void ReturnPlace()
    {
        EditMode = false;
        GridBuildIngData myBuildData = GridBuildSystem.Instance.GetGridBuildIngData(UID);
        if (myBuildData == null)
        {
            UnityPoolManager.Instance.Release(gameObject);
        }
        else
            transform.localPosition = GridBuildSystem.Instance.gridLayout.CellToLocalInterpolated(myBuildData.placedArea.position + new Vector3(0.5f, 0.5f, 0f));
        GridBuildSystem.Instance.ClearArea();
    }

    //건물 제거
    public void RemovePlace()
    {
        if (placed == false)
            return;
        GridBuildSystem.Instance.ClearArea();
        GridBuildIngData myBuildData = GridBuildSystem.Instance.GetGridBuildIngData(UID);
        if (myBuildData == null)
        {
            UnityPoolManager.Instance.Release(gameObject);
            return;
        }
        EditMode = false;
        GridBuildSystem.Instance.RemoveGridBuilding(area, UID);
        UnityPoolManager.Instance.Release(gameObject);
    }

    //npc와 건물과의 상호작용
    public void Interaction(EINTERACTION_BUILDING_TYPE type, InteractiveUnit unit, UnityAction onComplete = null)
    {
        if (buildingFunctionDic.ContainsKey(type) == false)
            return;
        if (eCurrentState == EBUILDING_STATE.Interaction)
            return;

        eCurrentState = EBUILDING_STATE.Interaction;
        buildingFunctionDic[type].Interaction(unit, () =>
        {
            onComplete?.Invoke();
            eCurrentState = EBUILDING_STATE.Idle;
        });
    }
    //건물을 클릭하면 건물 수정(옮기거나 제거)하는 기능을 수행하기 위한 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GridBuildSystem.Instance.isSelectedBuilding == true)
            return;

        if (eCurrentState == EBUILDING_STATE.Constrution)
            return;

        if (this is GatherBuilding == true)
            return;

        EditMode = true;
        placed = false;

        OpenbuildingEditMode();
    }

    //
    public void OpenbuildingEditMode()
    {
        buildingEditModeUI.Open(this);
        buildingEditModeUI.SetConfirmAction(Place);
        buildingEditModeUI.SetCancelAction(ReturnPlace);
        buildingEditModeUI.SetRemoveAction(RemovePlace);
    }

    
    //[Button("OnCompleteConstruction", ButtonSizes.Medium), TitleGroup("Building Cheat")]
    //public void OnCompleteConstruction()
    //{
    //    if (eCurrentState != EBUILDING_STATE.Constrution)
    //        return;
    //    foreach (var item in buildingProgress.requiredMaterials.Values)
    //    {
    //        item.submittedMaterialCount = item.requiredMaterialCount;
    //    }
    //    eCurrentState = EBUILDING_STATE.Idle;
    //}

    /////////////////////////////////
    ////////////////////////////////////////

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionRange);
    }

}
