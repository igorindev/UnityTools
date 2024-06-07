using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

[Overlay(typeof(SceneView), "Test")]
[Icon("Assets/unity.png")]
public class ToolbarMeshPivot : ToolbarOverlay
{
    public ToolbarMeshPivot() : base(AutoRefresh.id, FastPlayMode.id, CompilationMode.id, ReloadDomain.id)
    {
        //CreateContent(Layout.Panel);
    }
}