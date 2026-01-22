using UnityEngine;

public class UIStackDialogWindow : UIStackWindowBase
{
    public override void Open()
    {
        base.Open();
        stackWindow.Push(this);
    }
    
}
