using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Task
{
    public int priority;
    public EJOB_TYPE eJobType;
    public Transform owner;
    public Transform target;

    public InteractiveUnit worker;

    public TaskDetail taskDetail;
}
