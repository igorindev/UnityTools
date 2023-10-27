#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CustomPropertyDrawer(typeof(SearchAttribute))]
public class SearchAttributeDrawer : PropertyDrawer
{
    public static ObjectSearchProvider inst;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.width -= 60;
        EditorGUI.ObjectField(position, property, label);
        position.x += position.width;
        position.width = 60;

        if (GUI.Button(position, new GUIContent("Find")))
        {
            Type t = (attribute as SearchAttribute).searchObjectType;

            if (inst == null)
                inst = ScriptableObject.CreateInstance(nameof(ObjectSearchProvider)) as ObjectSearchProvider;

            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), inst.Initialize(t, property));
        }
    }
}
#endif