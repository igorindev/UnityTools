// /*---------------------------------------------------------------
// Author: ParanoidBigfoot
// Description: Add an option in the "Create" menu to create an asset from any ScriptableObject script.
// 		Useful for single instances of ScriptableObject (such as managers), without cluttering the menu.
// 		Just place in an Editor folder (Any folder named "Editor")
//
// GitHub: https://github.com/ParanoidBigfoot
// Twitter: @ParanoidBigfoot
// ---------------------------------------------------------------*/
using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateScriptableObjectContextMenu
{
    [MenuItem("Assets/Create ScriptableObject")]
    public static void CreateAsset()
    {
        MonoScript scriptFile = Selection.activeObject as MonoScript;
        Type t = scriptFile.GetClass();
        string path = AssetDatabase.GetAssetPath(scriptFile);
        path = Path.ChangeExtension(path, ".asset");

        path = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(t), path);
        UnityEngine.Object o = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        ProjectWindowUtil.ShowCreatedAsset(o);

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Assets/Create ScriptableObject", validate = true)]
    public static bool CreateAssetValidation()
    {
        UnityEngine.Object selected = Selection.activeObject;
        if (selected != null)
        {
            if (selected.GetType() == typeof(MonoScript))
            {
                MonoScript scriptFile = selected as MonoScript;
                Type t = scriptFile.GetClass();
                if (t != null)
                {
                    if (!t.IsAbstract)
                    {
                        if (t.IsSubclassOf(typeof(ScriptableObject)))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}