#if UNITY_2021_2_OR_NEWER
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.ShortcutManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

[EditorToolbarElement(id, typeof(SceneView))]
class AutoRefresh : EditorToolbarToggle
{
    public const string id = "Compilation/AutoRefresh";
    public string shortcut = ShortcutManager.instance.GetShortcutBinding("Main Menu/Assets/Refresh").ToString();
    public AutoRefresh()
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

[EditorToolbarElement(id, typeof(SceneView))]
class FastPlayMode : EditorToolbarToggle
{
    public const string id = "Compilation/FastPlayMode";
    public FastPlayMode()
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

[EditorToolbarElement(id, typeof(SceneView))]
class ReloadDomain : EditorToolbarButton
{
    public const string id = "Compilation/ReloadDomain";
    public ReloadDomain()
    {
        text = "Reload Domain";
        icon = EditorGUIUtility.IconContent("d_Refresh@2x").image as Texture2D;
        tooltip = "Reload all Scripts and Domain, reseting static variables. Use it when Fast Play (Disabled Domain Reload) is active.";
        clicked += OnClick;
    }

    void OnClick()
    {
        EditorUtility.RequestScriptReload();
    }

}

[EditorToolbarElement(id, typeof(SceneView))]
class CompilationMode : EditorToolbarDropdown
{
    public const string id = "Compilation/Dropdown";
    static int drop = EditorPrefs.GetInt("ScriptCompilationDuringPlay");

    public CompilationMode()
    {
        text = "Recompilation";
        tooltip = "Select when the scripts will be compiled.";
        clicked += ShowDropdown;
    }
    void ShowDropdown()
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Recompile And Continue Playing"), drop == 0, ScriptCompilationToggleOption0);
        menu.AddItem(new GUIContent("Recompile After Finished Playing"), drop == 1, ScriptCompilationToggleOption1);
        menu.AddItem(new GUIContent("Stop Playing And Recompile"), drop == 2, ScriptCompilationToggleOption2);
        menu.ShowAsContext();
    }
    public static void ScriptCompilationToggleOption0()
    {
        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 0);
        drop = 0;
    }
    public static void ScriptCompilationToggleOption1()
    {
        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 1);
        drop = 1;
    }
    public static void ScriptCompilationToggleOption2()
    {
        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 2);
        drop = 2;
    }
}

[Overlay(typeof(SceneView), "Compilation")]
[Icon("Assets/unity.png")]
public class CompilationToolbar : ToolbarOverlay
{
    CompilationToolbar() : base(AutoRefresh.id, FastPlayMode.id, CompilationMode.id, ReloadDomain.id)
    {
    }
}
#endif