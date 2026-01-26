using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class UIBuildingEditMode : UIWindowBase
{
    UnityAction confirmAction = null;
    UnityAction cancelAction = null;
    UnityAction removeAction = null;

    GridBuilding building;

    /*
     * *
     */

    public void Open(GridBuilding _building)
    {
        building = _building;
        base.Open();
    }
    public override void Close()
    {
        base.Close();
        confirmAction = null;
        cancelAction = null;
        removeAction = null;
        building = null;
    }
    
    public void OnClickConfirm()
    {
        confirmAction?.Invoke();
    }
    public void OnClickCancel()
    {
        cancelAction?.Invoke();
        Close();
    }
    public void OnClickRemove()
    {
        removeAction?.Invoke();
        Close();
    }

    public void SetConfirmAction(UnityAction confirmAction)
    {
        this.confirmAction = confirmAction;
    }
    public void SetCancelAction(UnityAction cancelAction)
    {
        this.cancelAction = cancelAction;
    }
    public void SetRemoveAction(UnityAction removeAction)
    {
        this.removeAction = removeAction;
    }

}