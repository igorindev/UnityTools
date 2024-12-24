using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimatorEventListener : MonoBehaviour
{
    [Serializable]
    public class AnimatorEventListenerTrigger
    {
        [SerializeField] private int hash;
        [SerializeField, Range(0, 1)] private float triggerTime;
        [SerializeField] private bool triggerOnlyOnce;
        [SerializeField] private UnityEvent onTrigger;

        private bool triggered;

        public int Hash => hash;

        public void Trigger(float currentTime)
        {
            if (triggerOnlyOnce && triggered)
            {
                return;
            }

            if (currentTime < triggerTime)
            {
                triggered = false;
            }

            if (triggered)
            {
                return;
            }

            if (currentTime >= triggerTime)
            {
                triggered = true;
                onTrigger?.Invoke();
            }
        }
    }

    [SerializeField] AnimatorEventListenerTrigger[] animatorEventListenerTrigger;
    Dictionary<int, List<AnimatorEventListenerTrigger>> animatorEvents;

    private void Start()
    {
        Setup();
    }

    internal void Setup()
    {
        animatorEvents ??= new Dictionary<int, List<AnimatorEventListenerTrigger>>();
        animatorEvents.Clear();

        foreach (var item in animatorEventListenerTrigger)
        {
            if (animatorEvents.ContainsKey(item.Hash))
            {
                animatorEvents[item.Hash].Add(item);
            }
            else
            {
                animatorEvents.Add(item.Hash, new List<AnimatorEventListenerTrigger>() { item });
            }
        }
    }

    internal void Dispatch(int hash, float currentNormalizedTime)
    {
        if (animatorEvents.TryGetValue(hash, out var events))
        {
            foreach (AnimatorEventListenerTrigger item in events)
            {
                item.Trigger(currentNormalizedTime);
            }
        }
    }
}
