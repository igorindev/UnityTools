using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Actions/Empty")]
public class ActionSO : ScriptableObject
{
    public bool debug = true;
#if UNITY_EDITOR
    [Multiline]
    public string developerDescription = "";
#endif
    public Action executeAction, stopAction;

    public virtual void Execute()
    {
        executeAction?.Invoke();
        DebugExecuteAction();
    }

    public virtual void Execute(GameObject gameObject)
    {
        executeAction?.Invoke();
        DebugExecuteAction();
    }

    public virtual void StopExecute()
    {
        stopAction?.Invoke();
        DebugStopAction();
    }

    public virtual void DebugExecuteAction()
    {
        if (debug)
        {
            Debug.LogWarning($"Execute: {name}");
        }
    }

    public virtual void DebugStopAction()
    {
        if (debug)
        {
            Debug.LogWarning($"Stopping: {name}");
        }
    }
}