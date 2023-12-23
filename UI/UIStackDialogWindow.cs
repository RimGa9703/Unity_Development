using UnityEngine;

public class UIStackDialogWindow : UIStackWindowBase
{
    public override void Open()
    {
        base.Open();
        stackWindow.Push(this);
    }
    //스택형 윈도우는 close가 곧 Free와 동일하게 작동해야함
    public override void Close()
    {
        foreach (var item in stackWindow)
        {
            item.Free();
        }
    }
    public override void Free()
    {
        foreach (var item in stackWindow)
        {
            item.Free();
        }
        stackWindow.Clear();
    }
}
