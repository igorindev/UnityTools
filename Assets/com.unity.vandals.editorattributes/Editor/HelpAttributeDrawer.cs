#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HelpAttribute))]
public class HelpAttributeDrawer : PropertyDrawer
{
    GUIStyle style = new GUIStyle(EditorStyles.helpBox);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(position, property.stringValue, style);
    }
}
#endif
