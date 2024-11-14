#if UNITY_2021_2_OR_NEWER
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

[Overlay(typeof(SceneView), "Compilation")]
[Icon("Assets/unity.png")]
public class CompilationToolbar : ToolbarOverlay
{
    CompilationToolbar() : base(AutoRefreshTool.id, FastPlayModeTool.id, CompilationModeTool.id, ReloadDomainTool.id)
    {
    }
}
#endif
