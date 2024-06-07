using System;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// An EditorTool that shows an Overlay in the active scene view while enabled.
[EditorTool("Overlay Tool Example", typeof(Transform))]
class ToolWithOverlay : EditorTool
{
    ActiveSceneViewOverlay m_Overlay;

    void OnEnable()
    {
        m_Overlay = new ActiveSceneViewOverlay(targets.Cast<Transform>().ToArray());
        SceneView.AddOverlayToActiveView(m_Overlay);
    }

    void OnDisable()
    {
        SceneView.RemoveOverlayFromActiveView(m_Overlay);
    }
}

// A simple Overlay that moves a collection of transforms by some translation.
class ActiveSceneViewOverlay : Overlay
{
    Vector3Field m_Translation;
    Transform[] m_Selection;

    public ActiveSceneViewOverlay(Transform[] selection)
    {
        m_Selection = selection;
    }

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        root.Add(m_Translation = new Vector3Field("Translation"));
        root.Add(new Button(MoveSelectionUp) { text = "Move Selection Up" });
        m_Translation.SetValueWithoutNotify(Vector3.up);
        m_Translation.style.minWidth = 300;
        return root;
    }

    void MoveSelectionUp()
    {
        Undo.RecordObjects(m_Selection.ToArray(), "Move Selection");
        foreach (var transform in m_Selection)
            transform.position += m_Translation.value;
    }
}