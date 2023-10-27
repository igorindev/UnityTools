#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class Divisor
{
    [MenuItem("GameObject/Divisor", false, 12)]
    static void CreateDivisior()
    {
        GameObject _object = new GameObject("_________________________________________________________________________________________");
        _object.tag = "EditorOnly";
    }
}
#endif