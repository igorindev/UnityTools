using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EventHolder", menuName = "ScriptableObjects/EventHolder")]
public class EventSO : ScriptableObject
{
    public List<UnityEventAndHolder> listeners;
    public void Raise()
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].unityEvent.Invoke();
        }
    }
}

[System.Serializable]
public class UnityEventAndHolder
{
    public UnityEventAndHolder(EventListener eventListener, UnityEvent callbacks)
    {
        holder = eventListener;
        unityEvent = callbacks;
    }

    public Object holder;
    public UnityEvent unityEvent;
}

public class EventListener : MonoBehaviour
{
    public EventSO eventSO;
    public UnityEvent callbacks;

    void OnEnable()
    {
        eventSO.listeners.Add(new UnityEventAndHolder(this, callbacks));
    }
}