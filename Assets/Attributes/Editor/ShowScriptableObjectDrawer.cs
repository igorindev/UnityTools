#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowScriptableObject))]
public class ShowScriptableObjectDrawer : PropertyDrawer
{
    Editor editor = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);

        if (property.objectReferenceValue != null)
        {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
        }

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            if (!editor)
            {
                Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
            }
            editor.OnInspectorGUI();

            EditorGUI.indentLevel--;
        }
    }
}
#endif
