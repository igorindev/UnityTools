using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Modules
{
    public abstract class Modular<T> where T : ModuleCollection
    {
        [SerializeField] protected List<Module> _modules;

        protected static Dictionary<Type, Module> activeModules;

        protected Modular()
        {
            T module = Resources.Load<T>(typeof(T).Name);
            if (module == null)
            {
                module = ScriptableObject.CreateInstance<T>();
            }

            module.Modules ??= new List<Module>();
            activeModules = module.Modules.ToDictionary(x => x.GetType());
        }

        public static bool TryGet<T1>(out T1 module) where T1 : Module
        {
            module = default;
            if (activeModules.TryGetValue(typeof(T1), out Module actualValue))
            {
                module = actualValue as T1;
                return true;
            }

            Debug.LogError($"Module of type {typeof(T1)} is not available");
            return false;
        }
    }

    public class Holder<T> where T : Module, new()
    {
        public Type Type => typeof(T);

        public T GetInstance() => new T();
    }

}
