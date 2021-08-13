using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ToolBar", menuName = "ScriptableObjects/Toolbar/Manager")]
public class ToolbarScriptableObject : ScriptableObject
{
    public Tools[] tools;
}

[System.Serializable]
public struct Tools
{
    public ToolbarTool tool;
    public string toolName;
    public string toolTip;
    public string toolIconPath;
}

public abstract class ToolbarTool : ScriptableObject
{
    public abstract void Perform();
}
