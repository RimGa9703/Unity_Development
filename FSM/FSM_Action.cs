using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class FSM_Action : ScriptableObject
{
    protected StateController stateController = null;
#if UNITY_EDITOR
    [TextArea(3, 5)]
    public string information = null;
#endif

    public abstract void EntryAction(StateController controller);
    public abstract bool UpdateAction(StateController controller);

    public abstract void FixedUpdateAction(StateController controller);
    public abstract void ExitAction(StateController controller);
}
