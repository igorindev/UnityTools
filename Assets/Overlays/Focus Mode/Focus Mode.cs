#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.ShortcutManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

[EditorToolbarElement(id, typeof(SceneView))]
class FocusModeActive : EditorToolbarToggle
{
    public const string id = "Focus/FocusMode";

    public FocusModeActive()
    {
        text = $" Focus: OFF";
        onIcon = EditorGUIUtility.IconContent("d_scenevis_visible_hover").image as Texture2D;
        offIcon = EditorGUIUtility.IconContent("d_scenevis_hidden_hover").image as Texture2D;
        tooltip = $"ON: Show only the GameObjects with correspondent layers in the Hierarchy.\nOFF: Show all GameObjects in the Hierarchy.";
        SetValueWithoutNotify(EditorPrefs.GetInt("kAutoRefresh") == 1);
        this.RegisterValueChangedCallback(Callback);
    }

    void Callback(ChangeEvent<bool> evt)
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

    public void Hide()
    {
        var ids = EditorPrefs.GetString("FocusModeID", "0,").Split(",").ToList();

        foreach (GameObject item in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (ids.Contains(item.layer.ToString()) == false)
                item.hideFlags = HideFlags.HideInHierarchy;
        }
    }
    public void ShowAll()
    {
        foreach (GameObject item in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (item.hideFlags != HideFlags.None)
                item.hideFlags = HideFlags.None;
        }
    }
}

[EditorToolbarElement(id, typeof(SceneView))]
class FocusLayers : EditorToolbarDropdown
{
    public const string id = "Focus/FocusLayers";
    static List<string> ids = new List<string>();

    FocusLayers()
    {
        ids = EditorPrefs.GetString("FocusModeID", "0").Split(",").ToList();
        if (ids.Count > 1)
        {
            text = $"Layer: Mixed...";
        }
        else
        {
            text = $"Layer: {LayerMask.LayerToName(int.Parse(ids[0]))}";
        }

        tooltip = "Select what layer the Hierachy will show.";
        clicked += ShowDropdown;
    }
    void ShowDropdown()
    { 
        ids = EditorPrefs.GetString("FocusModeID", "0").Split(",").ToList();

        var menu = new GenericMenu();

        for (int i = 0; i < 32; i++)
        {
            int id = i;
            string v = LayerMask.LayerToName(i);

            if (string.IsNullOrEmpty(v)) continue;

            bool alreadySelected = ids.Contains(id.ToString());
            menu.AddItem(new GUIContent(v), alreadySelected, () => FocusId(id.ToString(), !alreadySelected));
        }
        menu.ShowAsContext();
    }
    public void FocusId(string id, bool add)
    {
        if (add)
            ids.Add(id);
        else if (ids.Count - 1 != 0)
            ids.Remove(id);

        if (ids.Count > 1)
        {
            text = $"Layer: Mixed...";
        }
        else
        {
            text = $"Layer: {LayerMask.LayerToName(int.Parse(ids[0]))}";
        }

        string final = "";
        for (int i = 0; i < ids.Count; i++)
        {
            final += ids[i];

            if (i < ids.Count - 1)
                final += ",";
        }

        EditorPrefs.SetString("FocusModeID", final);
    }
}

[Overlay(typeof(SceneView), "Focus")]
[Icon("Assets/unity.png")]
public class FocusToolbar : ToolbarOverlay
{
    FocusToolbar() : base(FocusModeActive.id, FocusLayers.id) { }
}
#endif