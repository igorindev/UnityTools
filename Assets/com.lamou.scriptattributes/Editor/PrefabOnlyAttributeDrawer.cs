using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PrefabOnlyAttribute))]
public class PrefabOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.LabelField(position, label.text, "Use \"PrefabOnly\" on gameobjects and monobehaviours only.");
            return;
        }

        var go = property.objectReferenceValue as GameObject;
        if (go != null && go.scene.IsValid())
            property.objectReferenceValue = null;

        var comp = property.objectReferenceValue as Component;
        if (comp != null && comp.gameObject.scene.IsValid())
            property.objectReferenceValue = null;

        var obj = EditorGUI.ObjectField(position, label.text + " (Prafab)", property.objectReferenceValue, typeof(GameObject), false);
        property.objectReferenceValue = obj;
    }
}
