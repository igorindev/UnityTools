using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetReference : EditorWindow
{
    [SerializeField] Object asset;
    [SerializeField] List<Object> dependencies;

    static List<Object> assets;

    SerializedObject so;
    SerializedProperty assetProperty;

    static string myPath;
    static string[] paths;
    Vector2 scroll;

    static AssetReference window;

    [MenuItem("Assets/Show Asset(s) References...")]
    public static void GetReferences()
    {
        paths = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < assets.Count; i++)
        {
            Find(AssetDatabase.GetAssetPath(assets[i]));
        }
    }

    [MenuItem("Assets/Show Asset(s) References...", true)]
    public static bool CheckIfScriptFile()
    {
        assets = Selection.objects.ToList();

        if (assets != null && assets.Count > 0)
        {
            for (int i = 0; i < assets.Count; i++)
            {
                if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(assets[i])))
                    assets.RemoveAt(i);
            }
            return assets != null && assets.Count > 0;
        }
        return false;
    }

    static void Find(string path)
    {
        myPath = path;
        window = CreateInstance<AssetReference>();
        window.minSize = new Vector2(300, 300);
        window.Show();
    }

    void OnEnable()
    {
        so = new SerializedObject(this);
        asset = AssetDatabase.LoadAssetAtPath(myPath, typeof(Object));
        assetProperty = so.FindProperty("asset");

        dependencies = new List<Object>();
        
        for (int i = 0; i < paths.Length; i++)
        {
            if (AssetDatabase.IsValidFolder(paths[i]) || paths[i] == myPath) continue;

            string[] assetDependency = AssetDatabase.GetDependencies(paths[i]);

            for (int j = 0; j < assetDependency.Length; j++)
            {
                if (myPath == assetDependency[j])
                {
                    dependencies.Add(AssetDatabase.LoadAssetAtPath<Object>(paths[i]));
                }
            }
        }

        dependencies.Sort((p1, p2) => p1.name.CompareTo(p2.name));
    }

    void OnGUI()
    {
        so.Update();
        GUILayout.Space(6);
        EditorGUILayout.PropertyField(assetProperty);
        GUILayout.Space(10);
        EditorGUILayout.LabelField(dependencies.Count > 0 ? $"This asset is referenced by {dependencies.Count} assets:" : "This asset has no refereces.", EditorStyles.boldLabel);
        GUILayout.Space(2);
        using (var v = new EditorGUILayout.ScrollViewScope(scroll, EditorStyles.helpBox))
        {
            scroll = v.scrollPosition;
            for (int i = 0; i < dependencies.Count; i++)
            {
                EditorGUILayout.ObjectField(dependencies[i], typeof(Object), true);
            }
        }
    }
}
