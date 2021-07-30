using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tasks", menuName = "ScriptableObjects/Tasks")]
public class Tasks : ScriptableObject
{
    public List<Task> tasks = new List<Task>();
}

[System.Serializable]
public class Task
{
    [Scene] public string onScene;
    public string author;
    public string task;
    public bool completed;
    public Priority priority;

    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
