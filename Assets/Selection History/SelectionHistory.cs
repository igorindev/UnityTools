using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectionHistory : EditorWindow
{
    const int quantityOfItens = 25;
    EditorGUILayout.ScrollViewScope scrollView;
    Vector2 scroll;

    static List<Object> selections;
    static bool selectFolders = true;

    static SelectionHistory window;

    [MenuItem("Tools/Selection History...")]
    static void Init()
    {
        window = GetWindow<SelectionHistory>();
        window.minSize = new Vector2(200, 200);
    }

    void OnEnable()
    {
        selections = new List<Object>();
        Selection.selectionChanged += ChangeSelection;
    }
    void OnDestroy()
    {
        Selection.selectionChanged -= ChangeSelection;
    }

    void OnGUI()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            selectFolders = GUILayout.Toggle(selectFolders, "Select Folders");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear"))
            {
                selections.Clear();
            }
        }

        using (scrollView = new EditorGUILayout.ScrollViewScope(scroll, EditorStyles.helpBox))
        {
            var labelWidth = GUILayout.Width(50);
            var buttonWidth = GUILayout.Width(50);
            scroll = scrollView.scrollPosition;
            for (int i = selections.Count - 1; i >= 0; i--)
            {
                if (selections[i] == null) { selections.RemoveAt(i); break; }
                using (new EditorGUILayout.HorizontalScope())
                {
                    string v = i == selections.Count - 1 ? "Last" : (selections.Count - 1 - i).ToString();
                    EditorGUILayout.LabelField(v, labelWidth);
                    EditorGUILayout.ObjectField(selections[i], typeof(Object), true);
                }
            }
        }
    }

    void ChangeSelection()
    {
        Object active = Selection.activeObject;
        if(active == null) { return; }
        if (!selectFolders && AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(active)))
        {
            return;
        }

        if (selections.Contains(active))
            selections.Remove(active);

        selections.Add(active);

        if (selections.Count > quantityOfItens)
        {
            selections.RemoveAt(0);
        }

        Repaint();
    }
}
