#if UNITY_2021_2_OR_NEWER
using System.Linq;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

[EditorToolbarElement(id, typeof(SceneView))]
public class FocusModeTool : EditorToolbarToggle
{
    public const string id = "Focus/FocusMode";

    public FocusModeTool()
    {
        text = $" Focus: OFF";
        onIcon = EditorGUIUtility.IconContent("d_scenevis_visible_hover").image as Texture2D;
        offIcon = EditorGUIUtility.IconContent("d_scenevis_hidden_hover").image as Texture2D;
        tooltip = $"ON: Show only the GameObjects with correspondent layers in the Hierarchy.\nOFF: Show all GameObjects in the Hierarchy.";
        SetValueWithoutNotify(EditorPrefs.GetInt("kAutoRefresh") == 1);
        this.RegisterValueChangedCallback(Callback);
    }

    private void Callback(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            text = $" Focus: ON";
            Hide();
        }
        else
        {
            text = $" Focus: OFF";
            ShowAll();
        }
    }

    private void Hide()
    {
        var ids = EditorPrefs.GetString("FocusModeID", "0,").Split(",").ToList();

        foreach (GameObject item in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (ids.Contains(item.layer.ToString()) == false)
                item.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    private void ShowAll()
    {
        foreach (GameObject item in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (item.hideFlags != HideFlags.None)
            {
                item.hideFlags = HideFlags.None;
            }
        }
    }
}
#endif
