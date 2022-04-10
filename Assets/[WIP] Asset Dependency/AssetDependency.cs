using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using System.Linq;

public class AssetDependency : EditorWindow
{
    public string[] list;
    public List<AssetUsedBy> dependencies;
    [SerializeField] UnityEngine.Object asset;
    [SerializeField] AssetUsedBy usedBy;

    static List<Object> assets;

    SerializedObject so;
    SerializedProperty dependency;
    SerializedProperty assetProperty;

    static string myPath;

    [MenuItem("Assets/Get Dependencies...")]
    public static void GetReferences()
    {
        if (assets != null && assets.Count > 0)
        {
            for (int i = 0; i < assets.Count; i++)
            {
                Find(AssetDatabase.GetAssetPath(assets[i]));
            }
        }
    }

    [MenuItem("Assets/Get Dependencies...", true)]
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
        }
        return assets != null && assets.Count > 0;
    }

    static void Find(string path)
    {
        myPath = path;
        CreateWindow<AssetDependency>();
    }

    void OnEnable()
    {
        so = new SerializedObject(this);
        string path = myPath;
        asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        AssetUsedBy dependencie = new AssetUsedBy
        {
            asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object)),
        };

        dependencie.dependencies = new List<Object>();
        List<Object> list = new List<Object>();
        string[] paths = AssetDatabase.GetAllAssetPaths(); 

        for (int i = 0; i < paths.Length; i++)
        {
            if (AssetDatabase.IsValidFolder(paths[i])) continue;
            list.Add(AssetDatabase.LoadAssetAtPath<Object>(paths[i]));
        }

        for (int i = 0; i < list.Count; i++)
        {
            string[] v = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(list[i]));

            for (int j = 0; j < v.Length; j++)
            {
                if (path == v[j])
                {
                    dependencie.dependencies.Add(list[i]);
                }
            }
        }

        usedBy = dependencie;
        

    }

    void OnGUI()
    {
        so.Update();
        dependency = so.FindProperty("usedBy");
        assetProperty = so.FindProperty("asset");
        EditorGUILayout.PropertyField(assetProperty);
        EditorGUILayout.PropertyField(dependency);
    }


    [ContextMenu("Get")]
    void Get()
    {
        list = AssetDatabase.GetAllAssetPaths();

        Array.Sort(list, StringComparer.InvariantCulture);
        dependencies = new List<AssetUsedBy>();
        for (int i = 0; i < list.Length; i++)
        {
            if (AssetDatabase.IsValidFolder(list[i])) continue;

            AssetUsedBy dependencie = new AssetUsedBy
            {
                asset = AssetDatabase.LoadAssetAtPath(list[i], typeof(UnityEngine.Object)),
                // dependencies = AssetDatabase.GetDependencies(list[i]).ToList()
            };

            //dependencies.Add(dependencie);
        }
    }

    [Serializable]
    public struct AssetUsedBy
    {
        public UnityEngine.Object asset;
        public List<Object> dependencies;
    }
}
