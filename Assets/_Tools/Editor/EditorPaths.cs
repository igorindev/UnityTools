using UnityEditor;
using UnityEngine;

public class EditorPaths
{

    [MenuItem("Unity Explorer/Open Persitent Data Path")]
    private static void Open()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
