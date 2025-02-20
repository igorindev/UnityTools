using System;
using System.Collections.Generic;

public class Modular
{
    private static Dictionary<Type, SettingsModule> _settingsModules = new();

    internal static void AddModule<T>(SettingsModule settingsModule)
    {
        AddModule(typeof(T), settingsModule);
    }

    internal static bool AddModule<T>(Type type, T module) where T : SettingsModule
    {
        return _settingsModules.TryAdd(type, module);
    }

    internal static T Get<T>() where T : SettingsModule
    {
        if (_settingsModules.TryGetValue(typeof(T), out SettingsModule actualValue))
        {
            return actualValue as T;
        }

        return null;
    }
}
