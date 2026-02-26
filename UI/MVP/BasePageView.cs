using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
public class BasePageView : BaseMVPView, IWindowView
{
    public event UnityAction confirmAction;
    public event UnityAction cancelAction;

    public BaseWindowPresenter presenter;

    public override void Free()
    {
        base.Free();
        
        confirmAction = null;
        cancelAction = null;
    }

    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
        
    }
    public void OnClickConfirm()
    {
    }
    public void OnClickCancel() 
    {
    }
}
