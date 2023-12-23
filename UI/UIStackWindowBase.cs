using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStackWindowBase : UIWindowBase
{
    protected Stack<UIWindowBase> stackWindow = new Stack<UIWindowBase>();

    /*
    * *
    */

    public override void Open()
    {
        base.Open();
        
    }

    public override void Free()
    {
        base.Free();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIWindowBase windowBase = stackWindow.Pop();
            if(windowBase != null)
            {
                windowBase.Free();          
            }
        }
    }
}
