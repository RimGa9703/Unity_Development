using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UIMainWindow : UIWindowBase
{
    public List<UIWindowBase> childWindows = new List<UIWindowBase>();


    public void AddChild(UIWindowBase child)
    {
        if (childWindows.Contains(child) == true)
            return;
        childWindows.Add(child);
    }
    public override void Free()
    {
        base.Free();
        for (int i = 0; i < childWindows.Count; i++)
        {
            childWindows[i].Free();
        }
    }
    public override void Refresh()
    {
        base.Refresh();
        for (int i = 0; i < childWindows.Count; i++)
        {
            childWindows[i].Refresh();
        }
    }

}
