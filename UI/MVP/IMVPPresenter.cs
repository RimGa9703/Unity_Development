/// <summary>
/// 모든 Presenter가 구현해야 하는 기본 인터페이스
/// View와 Model 사이의 징검다리 역할을 정의합니다.
/// </summary>
public interface IMVPPresenter
{
    void Init();
    void Free();

    void Open();

    void Close();
    
}