using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

    public class UITabButton : UIButtonBase
    {
        UITabGroup group = null;
        public int index = 0;

        [SerializeField]
        protected GameObject enableTab = null;

        [SerializeField]
        protected GameObject disableTab = null;

        [SerializeField]
        protected UILabel txtTitle = null;

        public bool isLock = false;

        [HideInInspector]
        public UnityAction notifyForLockAction = null;
        /*
         * *
         */

        public void InitInstance(UITabGroup _group, int _index)
        {
            group = _group;
            index = _index;
        }
        public void SetTabTitle(string title)
        {
            if(txtTitle != null) txtTitle.text = title;
        }

        public void SetEnableTap(bool flag)
        {
            if(enableTab != null) enableTab.SetActive(flag);
            if (disableTab != null) disableTab.SetActive(!flag); // 반전
        }

        public override void OnClick()
        {
            if (enableTab != null && enableTab.activeInHierarchy == true)
                return;
            if (disableTab != null && disableTab.activeInHierarchy == false)
                return;

            base.OnClick();
            PlaySFX();
            if (isLock == false)
                group.Set(this);
            else
            {
                if (notifyForLockAction != null)
                    notifyForLockAction.Invoke();
            }
        }
        public void SetNotifyForLockAction(UnityAction action)
        {
            notifyForLockAction = action;
        }
    }