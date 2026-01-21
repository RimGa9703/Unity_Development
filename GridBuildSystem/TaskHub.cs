using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TaskHub : MonoSingleton<TaskHub> 
{
    [ShowInInspector]
    public List<Task> globalTaskQueue = new List<Task>();

    public void AddTask(Task task)
    {
        globalTaskQueue.Add(task);
    }

    public Task GetAvailableTask(Func<Task, bool> predicate)
    {
        Task task = globalTaskQueue.Find(new Predicate<Task>(predicate)); // 내부적으로만 변환
        RemoveTask(task);
        return task;
    }
    public void GenarateTask(Transform owner, Transform target, TaskDetail taskDetail, EJOB_TYPE eJOB_TYPE)
    {
        Task task = new Task();
        task.priority = 0;
        task.owner = owner;

        task.eJobType = eJOB_TYPE;
        task.target = target;
        task.taskDetail = taskDetail;
        AddTask(task);
    }


    public List<Task> GetTasks() => globalTaskQueue;
    public void RemoveTask(Task task) => globalTaskQueue.Remove(task);
}
