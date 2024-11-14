using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

[EditorToolbarElement(id, typeof(SceneView))]
internal class FastPlayModeTool : EditorToolbarToggle
{
    public const string id = "Compilation/FastPlayMode";
    public FastPlayModeTool()
    {
        text = EditorSettings.enterPlayModeOptionsEnabled ? "FastPlay: ON" : "FastPlay: OFF";
        onIcon = EditorGUIUtility.IconContent("d_Animation.LastKey").image as Texture2D;
        offIcon = EditorGUIUtility.IconContent("d_Animation.Play").image as Texture2D;
        tooltip = "ON: Disable Domain Reload; enter PlayMode extremely fast. ATTENTION: Static variables are persistent per play, use Reload Domain to reset them." +
                  "\nOFF: Normal PlayMode conditions.";
        SetValueWithoutNotify(EditorSettings.enterPlayModeOptionsEnabled);
        this.RegisterValueChangedCallback(Callback);
    }

    void Callback(ChangeEvent<bool> evt)
    {
        EditorSettings.enterPlayModeOptionsEnabled = evt.newValue;
        if (evt.newValue)
        {
            text = "FastPlay: ON";
        }
        else
        {
            text = "FastPlay: OFF";
        }
    }
}
