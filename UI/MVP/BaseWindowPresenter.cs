using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseWindowPresenter : BaseMVPPresenter<IWindowView>
{
    //private readonly GameData model;

    private readonly UnityAction confirmAction;
    private readonly UnityAction cancelAction;

    //매개변수에 model 추가하기
    public BaseWindowPresenter(IWindowView view,UnityAction _confirmAction,UnityAction _cancelAction) : base(view)
    {   
        confirmAction = _confirmAction;
        cancelAction = _cancelAction;
    }

    public override void Free()
    {
        View.confirmAction -= OnConfirmAction;
        View.cancelAction -= OnCancelAction;
    }

    //Init에서 이벤트 구독 로직 수행
    public override void Init()
    {
        View.confirmAction += OnConfirmAction;
        View.cancelAction += OnCancelAction;
    }
    public override void Open()
    {
        View.Open();
    }
    public override void Close()
    {
        View.Close();
    }
    public virtual void Refresh()
    {
        View.Refresh();
    }

    void OnConfirmAction() 
    {
        confirmAction?.Invoke();
    }
    void OnCancelAction()
    {
        cancelAction?.Invoke();
    }
}
