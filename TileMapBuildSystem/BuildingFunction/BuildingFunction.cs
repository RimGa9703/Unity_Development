using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Define;

public abstract class BuildingFunction : ScriptableObject
{
    protected GridBuilding myBuilding;
    
    /*
     * *
     */

    public virtual void Init(GridBuilding _gridBuilding, EINTERACTION_BUILDING_TYPE _type)
    {
        myBuilding = _gridBuilding;
    }
    public virtual void Interaction(InteractiveUnit unit, UnityAction onComplete = null)
    {
        
    }
}
