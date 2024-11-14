using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

[EditorToolbarElement(id, typeof(SceneView))]
internal class AutoRefreshTool : EditorToolbarToggle
{
    public const string id = "Compilation/AutoRefresh";
    public string shortcut = ShortcutManager.instance.GetShortcutBinding("Main Menu/Assets/Refresh").ToString();
    public AutoRefreshTool()
    {
        text = EditorPrefs.GetInt("kAutoRefresh") == 1 ? "Refresh: Auto" : $"Refresh: {shortcut}";
        onIcon = EditorGUIUtility.IconContent("d_preAudioAutoPlayOff").image as Texture2D;
        offIcon = EditorGUIUtility.IconContent("d_scenepicking_pickable_hover").image as Texture2D;
        tooltip = $"ON: Auto compile scripts.\nOFF: Manually compile with {shortcut} shortcut.";
        SetValueWithoutNotify(EditorPrefs.GetInt("kAutoRefresh") == 1);
        this.RegisterValueChangedCallback(Callback);
    }

    void Callback(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            text = "Refresh: Auto";
            EditorPrefs.SetInt("kAutoRefresh", 1);
        }
        else
        {
            text = $"Refresh: {shortcut}";
            EditorPrefs.SetInt("kAutoRefresh", 0);
        }
    }
}
