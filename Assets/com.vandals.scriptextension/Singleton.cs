﻿using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    public bool dontDestroyOnLoad = true;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance) return instance;
            return instance = FindObjectOfType<T>();
        }
    }

    protected virtual void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;
            
            if (dontDestroyOnLoad) 
                DontDestroyOnLoad(gameObject);
        }
    }
}