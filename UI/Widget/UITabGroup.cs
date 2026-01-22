using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UITabGroup : UIWidgetBase
{
    public UITabButton[] tabs;
    public UITabButton startingTap = null;

    public bool useEnableReset = true;
    UITabButton currentTab = null;
    public UITabButton CurrentTab { get { return currentTab; } }
    public UnityEvent changeEvent = null;

    /*
     * *
     */

    void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].InitInstance(this, i);
        }
    }

    void OnEnable()
    {
        if (useEnableReset == false)
            return;
        OnReset();
    }

    public override void Refresh() { }

    public void OnReset()
    {
        if (startingTap == null)
            return;

        Set(startingTap);
    }

    public void Set(UITabButton _tap)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].Equals(_tap) == false)
            {
                tabs[i].SetEnableTap(false);
                continue;
            }

            tabs[i].SetEnableTap(true);
            currentTab = tabs[i];

            changeEvent.Invoke();
        }
    }

    public void Set(int _index, bool needEvent = true)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].index != _index)
            {
                tabs[i].SetEnableTap(false);
                continue;
            }

            tabs[i].SetEnableTap(true);
            currentTab = tabs[i];

            if (needEvent == true)
                changeEvent.Invoke();
        }
    }

    public void Hide(int _index)
    {
        tabs[_index].gameObject.SetActive(false);
    }
    public void Show(int _index)
    {
        tabs[_index].gameObject.SetActive(true);
    }
}
