using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildPlayerOptionsSO", menuName = "ScriptableObjects/BuildPlayerOptions", order = 1)]
public class BuildPlayerOptionsSO : ScriptableObject
{
    public BuildPlayerOptionsInspector[] builds;
}
