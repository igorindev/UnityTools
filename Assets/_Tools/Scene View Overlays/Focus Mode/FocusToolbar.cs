#if UNITY_2021_2_OR_NEWER
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

[Overlay(typeof(SceneView), "Focus")]
[Icon("Assets/unity.png")]
public class FocusToolbar : ToolbarOverlay
{
    FocusToolbar() : base(FocusModeTool.id, FocusLayersTool.id) { }
}
#endif
