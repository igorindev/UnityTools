using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class ShowAllAssetsReferences : EditorWindow
{
    static List<Object> dependencies = new List<Object>();
    static List<Object> assets = new List<Object>();
    static List<string> tempPaths = new List<string>();

    SerializedObject so;
    SerializedProperty assetProperty;

    static string myPath;
    static string[] paths;
    static string[] fixedPaths;
    Vector2 scroll;

    static ShowAllAssetsReferences window;

    AnimBool showExtraField;

    [MenuItem("Assets/Show All Assets References...")]
    public static void GetReferences()
    {
        paths = AssetDatabase.GetAllAssetPaths();
        assets.Clear();
        tempPaths.Clear();

        for (int i = 0; i < paths.Length; i++)
        {
            string ext = Path.GetExtension(paths[i]);
            Debug.Log(ext);
            if (AssetDatabase.IsValidFolder(paths[i])) continue;
            if (ext == "pdf") continue;

            assets.Add(AssetDatabase.LoadAssetAtPath<Object>(paths[i]));
            tempPaths.Add(paths[i]);
        }

        fixedPaths = tempPaths.ToArray();

        Open();
    }

    static void Open()
    {
        window = CreateInstance<ShowAllAssetsReferences>();
        window.minSize = new Vector2(600, 600);
        window.Show();
    }

    void OnEnable()
    {
        //showExtraField = new AnimBool(true);
        //showExtraField.valueChanged.AddListener(new UnityAction(base.Repaint));

        so = new SerializedObject(this);
    }

    Object[] FindDependencies(string myPath)
    {
        dependencies.Clear();

        for (int i = 0; i < fixedPaths.Length; i++)
        {
            if (AssetDatabase.IsValidFolder(fixedPaths[i]) || fixedPaths[i] == myPath) continue;

            string[] assetDependency = AssetDatabase.GetDependencies(fixedPaths[i]);

            for (int j = 0; j < assetDependency.Length; j++)
            {
                if (myPath == assetDependency[j])
                {
                    dependencies.Add(AssetDatabase.LoadAssetAtPath<Object>(fixedPaths[i]));
                }
            }
        }

        dependencies.Sort((p1, p2) => p1.name.CompareTo(p2.name));
        return dependencies.ToArray();
    }

    void OnGUI()
    {
        so.Update();
        GUILayout.Space(6);

        using (var v = new EditorGUILayout.ScrollViewScope(scroll, EditorStyles.helpBox))
        {
            scroll = v.scrollPosition;
            for (int i = 0; i < assets.Count; i++)
            {
                EditorGUILayout.ObjectField(assets[i], typeof(GameObject), true);

                //using (var group = new EditorGUILayout.FadeGroupScope(showExtraField.faded))
                {
                    //if (group.visible)
                    {
                        EditorGUI.indentLevel++;

                        Object[] objects = FindDependencies(fixedPaths[i]);

                        for (int j = 0; j < objects.Length; j++)
                        {
                            EditorGUILayout.ObjectField(objects[j], typeof(Object), true);
                        }

                        EditorGUI.indentLevel--;
                    }
                }
            }
        }
    }
}
