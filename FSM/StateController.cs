using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEditor;
using Only1Games.Util;
using Unity.Collections;
using Pathfinding;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class StateController : MonoBehaviour
{
    public Animator animator = null;
    int[] parameterNames = null;

    [SerializeField] State currentState = null;
    [SerializeField] State idleState = null;

    [Searchable, TitleGroup("FSM Setting/FSM"), TableList]
    public List<StateInfo> states = new List<StateInfo>();
    
    /*
     * *
     */


    public void OnEnable()
    {
        int Count = states.Count;
        for (int i = 0; i < Count; i++)
        {
            states[i].enable = true;
        }

    }

    public void DisenableAllStates()
    {
        int Count = states.Count;
        for (int i = 0; i < Count; i++)
        {
            states[i].enable = false;
        }
    }

    public void EnableAllStates()
    {
        int Count = states.Count;
        for (int i = 0; i < Count; i++)
        {
            states[i].enable = true;
        }
    }
    // Call from Character
    public void Recycle()
    {
        currentState = idleState;
        if (parameterNames == null || parameterNames.Length != animator.parameters.Length)
            parameterNames = new int[animator.parameters.Length];
        for (int i = 0; i < parameterNames.Length; i++)
            parameterNames[i] = Animator.StringToHash(animator.parameters[i].name);
        OffAllAnimatorParam();
        animator.Rebind();
    }

    public void Release()
    {
        currentState = idleState;
        animator.Rebind();
    }
    public void Update()
    {
        if (currentState.Equals(idleState) == true)
        {
            ChangeToBestState();
            return;
        }

        bool checkUpdate = currentState.UpdateAction(this);
        if (checkUpdate == false)
        {
            ChangeToBestState();
            return;
        }
        checkUpdate = currentState.CheckUpdate(this);
        if (checkUpdate == false)
        {
            ChangeToBestState();
            return;
        }
    }

    public void FixedUpdate()
    {
        if (currentState == null)
            return;
        if (currentState.isUseFixedUpdate == true)
            currentState.FixedUpdateAction(this);
    }

    void ChangeToBestState()
    {
        StateInfo bestStateInfo = GetBestNextStateInfo();
        if (bestStateInfo == null) // 특별한 조건이 없을 경우 idleState로 이동한다
        {
            ChangeToState(idleState);
            return;
        }

        ChangeToState(bestStateInfo.state);
    }

    public void ChangeToState(State nextState)
    {
        // 변경되어야할 state가 같을 경우에도 EntryActions가 호출되어야 한다
        if (nextState == null) // || currentState.Equals(nextState) == true)
            return;
        if (currentState.Equals(nextState) == true)
        {
            OnExitState();
            currentState.EntryActions(this);
            return;
        }

        OnExitState();
        currentState = nextState;
        currentState.EntryActions(this);
    }
    void OnExitState()
    {
        currentState.ExitAction(this);
        OffAllAnimatorParam();
    }
    public void OffAllAnimatorParam()
    {
        if (animator == null || animator.parameters == null || parameterNames == null)
            return;
        animator.speed = 1;

        int animatorParamCount = parameterNames.Length;
        for (int i = 0; i < animatorParamCount; i++)
        {
            animator.SetBool(parameterNames[i], false);
            animator.SetInteger(parameterNames[i], 0);
        }
    }

    public void SetAnimController(RuntimeAnimatorController _controller)
    {
        animator.runtimeAnimatorController = _controller;
    }

    ////////////////////////////////////////////////////////////
    //
    // 다음 최선의 State를 찾는다
    StateInfo GetBestNextStateInfo()
    {
        StateInfo bestStateInfo = null;

        StateInfo stateInfo = null;
        int Count = states.Count;
        for (int i = 0; i < Count; i++)
        {
            stateInfo = states[i];

            if (stateInfo.enable == false)
                continue;

            if (stateInfo.state.CheckEntry(this) == false) //조건을 만족하는가?
                continue;

            /**/
            if (bestStateInfo == null) // 최초 bestState
            {
                bestStateInfo = stateInfo;
                continue;
            }

            /**/
            if (bestStateInfo.priority < stateInfo.priority)
                continue;
            bestStateInfo = stateInfo;
        }
        return bestStateInfo;
    }

    public StateInfo FindStateInfo(string name)
    {
        for (int i = 0; i < states.Count; i++)
        {
            if (states[i].state.name.Equals(name) == false)
                continue;
            return states[i];
        }

        return null;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (currentState == null)
            return;
        //Selection

        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.red;
        Handles.Label(transform.position, currentState.name, style);
    }
#endif
}
