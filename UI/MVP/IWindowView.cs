using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IWindowView : IMVPView
{
    
    event UnityAction confirmAction;
    event UnityAction cancelAction;
}
