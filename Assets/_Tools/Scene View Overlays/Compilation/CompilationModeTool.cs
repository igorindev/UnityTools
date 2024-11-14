using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

[EditorToolbarElement(id, typeof(SceneView))]
internal class CompilationModeTool : EditorToolbarDropdown
{
    public const string id = "Compilation/Dropdown";
    static int drop = EditorPrefs.GetInt("ScriptCompilationDuringPlay");

    public CompilationModeTool()
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
