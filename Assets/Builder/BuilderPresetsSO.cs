using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildPlayerOptionsSO", menuName = "ScriptableObjects/BuildPlayerOptions", order = 1)]
public class BuilderPresetsSO : ScriptableObject
{
    public BuildPlayerOptionsInspector[] builds;
    public List<string> history = new List<string>();
    public string currentBuild;

    [ContextMenu("Reset")]
    void Reset()
    {
        history.Clear();
    }
}