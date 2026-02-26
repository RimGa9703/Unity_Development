/// <summary>
/// 모든 View가 구현해야 하는 기본 인터페이스
/// </summary>
public interface IMVPView
{
    bool IsOpen { get; }

    void Open();
    void Close();
    void Free();

    void Refresh();
    
}