using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowBase : MonoBehaviour
{
    public string uiName = null;
    private bool isOpen = false;
    public bool IsOpen
    {
        get { return isOpen; }
        set { isOpen = value; }
    }

    public virtual void Open()
    {
        IsOpen = true;
        gameObject.SetActive(true);
    }
    public virtual void Close()
    {
        IsOpen = false;
        gameObject.SetActive(false);
    }
    public virtual void Free()
    {
        if (IsOpen == true)
            Close();
        UnityPoolManager.Instance.Release(gameObject);
    }
    public virtual void Refresh()
    {

    }
}
