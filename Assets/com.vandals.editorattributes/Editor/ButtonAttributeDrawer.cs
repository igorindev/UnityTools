using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonAttributeDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var obj = (MonoBehaviour)target;
        AddButtons(obj);
    }

    public void AddButtons(UnityEngine.Object dependant)
    {
        Type type = dependant.GetType();
        while (type != null)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            bool added = false;
            
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<ButtonAttribute>() != null)
                {
                    if (added == false)
                    {
                        added = true;
                        GUILayout.Space(15);
                        GUILayout.Label("Methods:");
                    }

                    if (GUILayout.Button(method.Name + "()"))
                    {
                        method.Invoke(dependant, null);
                        EditorUtility.SetDirty(dependant);
                    }
                }
            }
            type = type.BaseType;
        }
    }
}