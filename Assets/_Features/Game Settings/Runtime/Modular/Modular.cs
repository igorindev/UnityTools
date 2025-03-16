using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules
{
    public class Modular
    {
        internal Dictionary<Type, Module> _modules = new();
    }

    public class Modular<T> : Modular where T : Modular, new()
    {
        private static T s_instance;

        public Modular()
        {
            s_instance = this as T;
        }

        internal static void AddModule<T1>(Module settingsModule)
        {
            AddModule(typeof(T1), settingsModule);
        }

        internal static bool AddModule<T1>(Type type, T1 module) where T1 : Module
        {
            return s_instance._modules.TryAdd(type, module);
        }

        internal static T1 Get<T1>() where T1 : Module
        {
            if (s_instance._modules.TryGetValue(typeof(T1), out Module actualValue))
            {
                return actualValue as T1;
            }

            Debug.LogError($"Module of type {typeof(T1)} is not available for {typeof(T)}");
            return null;
        }
    }
}
