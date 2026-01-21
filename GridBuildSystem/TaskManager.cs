using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TaskManager
{
    [SerializeField] List<Task> taskList = new();

    public void Init()
    {
        taskList.Clear();
    }
    public void AddTask(Task task)
    {
        if (taskList.Contains(task) == false)
        {
            taskList.Add(task);
            taskList.Sort((a, b) => b.priority.CompareTo(a.priority));
        }
    }

    public Task GetTask(EJOB_TYPE type)
    {
        if (taskList.Count == 0)
            return null;

        //RemoveTask(task);
        return taskList.Find(a => a.eJobType.Equals(type) == true);
    }
    public Task GetTask()
    {
        if (taskList.Count == 0)
            return null;

        // priority가 가장 낮은 Task는 리스트의 마지막에 위치 (Add 시 Sort 했기 때문)
        Task task = taskList[taskList.Count - 1];
        //RemoveTask(task);
        return task;
    }
    public List<Task> GetRunningTasks()
    {
        return taskList.FindAll(a => a.worker != null);
    }
    public List<Task> GetWaitingTasks()
    {
        return taskList.FindAll(a => a.worker == null);
    }

    public void RemoveTask(Task task)
    {
        if(taskList.Contains(task) == true)
            taskList.Remove(task);
    }
}
