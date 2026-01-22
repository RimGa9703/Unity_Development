using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIButtonBase : MonoBehaviour
{
    [SerializeField]
    public GameObject gray = null;
    public bool IsGray { get { return isGray; } }
    protected bool isGray = false;

    protected UnityAction clickAction = null;
    protected UnityAction grayAction = null;

    /*
     * *
     */

    public virtual void OnClick()
    {
        if (enabled == false)
            return;
        if (gray != null && gray.activeInHierarchy == true && grayAction != null)
        {
            grayAction.Invoke();
            return;
        }
        if (clickAction != null)
            clickAction.Invoke();
    }

    public virtual void SetClickAction(UnityAction _clickAction)
    {
        clickAction = _clickAction;
    }

    public void SetGray(bool _value, UnityAction _action = null)
    {
        isGray = _value;
        if (gray == null)
        {
            grayAction = null;
            return;
        }
        gray.SetActive(isGray);
        grayAction = _action;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}

