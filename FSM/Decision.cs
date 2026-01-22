using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "FSM/Decision")]
public abstract class Decision : ScriptableObject
{
#if UNITY_EDITOR
    [TextArea(3, 5)]
    public string information = null;
#endif

    public abstract bool Decide(StateController controller);
}
