using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

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