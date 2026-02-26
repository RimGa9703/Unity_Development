using UnityEngine;

/// <summary>
/// 모든 View의 베이스 클래스
/// IView 인터페이스를 구현하며 공통 Open/Close/Free/Refresh 로직을 담당합니다.
/// 각 View는 이 클래스를 상속받아 필요한 UI 요소만 추가합니다.
/// </summary>

public abstract class BaseMVPView : MonoBehaviour, IMVPView
{
    public bool IsOpen => gameObject.activeSelf;

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
    public virtual void Free()
    {

    }
    public virtual void Refresh()
    {

    }
}
