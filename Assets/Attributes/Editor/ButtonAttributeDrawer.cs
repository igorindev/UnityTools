#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the ButtonAttribute.
        ButtonAttribute buttonAttribute = ((ButtonAttribute)this.fieldInfo.GetCustomAttribute(typeof(ButtonAttribute)));

        // Draw the button.
        if (GUI.Button(new Rect(18, 10, position.width, 18), label.text))
        {
            // If the button is clicked call the method from the attribute.
            buttonAttribute.Callback();
        }
    }
}
#endif