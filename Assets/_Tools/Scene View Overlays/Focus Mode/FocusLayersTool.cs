#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

[EditorToolbarElement(id, typeof(SceneView))]
public class FocusLayersTool : EditorToolbarDropdown
{
    public const string id = "Focus/FocusLayers";
    static List<string> ids = new List<string>();

    FocusLayersTool()
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
#endif
