using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateInfo
{
    /**/
    //State 작동 여부
    public bool enable = true;
    //우선 순위
    public int priority = 0; 
    public FSM_State state = null;
}
