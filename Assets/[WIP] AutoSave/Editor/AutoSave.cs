using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetsIconByType : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            Debug.Log("Reimported Asset: " + str);
            //EditorGUIUtility.SetIconForObject(AssetDatabase.LoadAssetAtPath<Object>(importedAssets[0]), (Texture2D)EditorGUIUtility.IconContent("BoxCollider Icon").image);
        }
        foreach (string str in deletedAssets)
        {
            Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
    }
}