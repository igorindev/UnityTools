// This script allows you to associate UnityEvents with trigger events in 2D space.
// You can specify actions to be triggered when a collider with a required tag enters, exits, or stays within a trigger zone.

using UnityEngine;
using UnityEngine.Events;

public class OnTrigger2DUnityAction : MonoBehaviour
{
    public UnityEvent onTriggerEnter2DAction, onTriggerExit2DAction, onTriggerStay2DAction;
    public StringReference requiredTag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enabled && other.CompareTag(requiredTag.Value))
        {
            onTriggerEnter2DAction?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (enabled && other.CompareTag(requiredTag.Value))
        {
            onTriggerExit2DAction?.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (enabled && other.CompareTag(requiredTag.Value))
        {
            onTriggerStay2DAction?.Invoke();
        }
    }
}