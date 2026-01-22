using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/State")]
public class FSM_State : ScriptableObject
{
    public bool isHold = false;
    public bool isUseUpdateCheck = true;
    public bool isUseFixedUpdate = false;

    [System.Serializable]
    public class DecisionData
    {
        [HorizontalGroup]
        public Decision decision;
        [HorizontalGroup, ValueDropdown("TrueOrFalse"), HideLabel]
        public bool isNOT;

        private IEnumerable TrueOrFalse = new ValueDropdownList<bool>()
            {
                { "True", true },
                { "False", false },
            };
    }

    public DecisionData[] entryDecisions = null;
    public DecisionData[] updateDecisions = null;
    //public DecisionData[] exitDecisions = null;

    public Action[] actions = null;

#if UNITY_EDITOR
    [TextArea(3, 5), Space(10)]
    public string information = "[information]";
#endif

    /*
     * *
     */

    // 다수의 조건 중에 하나라도 불만족일 경우 false
    public bool CheckEntry(StateController controller)
    {
        bool flag = false;
        DecisionData data;
        int Count = entryDecisions.Length;
        for (int i = 0; i < Count; i++)
        {
            data = entryDecisions[i];

            if (data.isNOT == true)
                flag = data.decision.Decide(controller);
            else
                flag = !data.decision.Decide(controller);

            if (flag == false)
                return false;
        }

        return true;
    }
    public bool CheckUpdate(StateController controller)
    {
        if (isUseUpdateCheck == false)
            return true;
        if (updateDecisions.Length <= 0)
            return false;

        bool flag = false;
        DecisionData data;
        int Count = updateDecisions.Length;
        for (int i = 0; i < Count; i++)
        {
            data = updateDecisions[i];

            if (data.isNOT == true)
                flag = data.decision.Decide(controller);
            else
                flag = !data.decision.Decide(controller);

            if (flag == false)
                return false;
        }

        return true;
    }

    //////////////////////////////////////////////////////////////////////
    //
    public void EntryActions(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].EntryAction(controller);
        }
    }

    public bool UpdateAction(StateController controller)
    {
        bool checkUpdate = true;
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i].UpdateAction(controller) == false)
                checkUpdate = false;
        }
        return checkUpdate;
    }
    public void FixedUpdateAction(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].FixedUpdateAction(controller);
        }
    }

    public void ExitAction(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].ExitAction(controller);
        }
    }
}
