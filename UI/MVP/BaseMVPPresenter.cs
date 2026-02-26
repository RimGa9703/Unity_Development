using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseMVPPresenter<TMVPView> : IMVPPresenter where TMVPView : IMVPView
{

    protected TMVPView View { get; private set; }

    protected BaseMVPPresenter(TMVPView view)
    {
        View = view;
    }

    /// <summary>
    /// View 이벤트 구독 해제 등 정리 작업을 여기서 처리
    /// </summary>
    public abstract void Free();


    /// <summary>
    /// View 이벤트 구독 등 초기화 작업을 여기서 처리
    /// </summary>
    public abstract void Init();

    public abstract void Open();

    public abstract void Close();
    
}
