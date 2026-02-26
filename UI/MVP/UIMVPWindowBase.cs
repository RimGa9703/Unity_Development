using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMVPWindowBase : MonoBehaviour
{
    public BasePageView windowView;
    public bool isOpen => windowView.IsOpen;
    
    public virtual void Free()
    {

    }
    public virtual void Init()
    {
        //UIMVPWindowBase를 상속 받은 클래스에서 override Init()타이밍에 BaseWindowPresenter 생성하기
        //BaseWindowPresenter생성 되면 BaseWindowPresenter.init();
    }

    public virtual void Open()
    {
        //BaseWindowPresenter.Open() 선언
    }
    public virtual void Close()
    {
        //BaseWindowPresenter.Close() 선언
    }

    public virtual void Refresh()
    {
        //BaseWindowPresenter.Refresh() 선언
    }
}
