using System.Collections.Generic;
using Modules;
using UnityEngine;

public class ModuleCollection : ScriptableObject
{
    public List<Module> Modules;
}

[CreateAssetMenu(menuName = "Settings/Video")]
public class VideoSettingsModuleCollection : ModuleCollection
{
}
