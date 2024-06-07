using UnityEditor;
using UnityEngine;

namespace ExtraContext
{
    public class ExtraContextVector3 : MonoBehaviour
    {
        [InitializeOnLoadMethod]
        public static void Init()
        {
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Vector3)
                return;

            menu.AddItem(new GUIContent("Set (0, 0, 0)"), false, () =>
            {
                property.vector3Value = Vector3.zero;
                property.serializedObject.ApplyModifiedProperties();
            });

            menu.AddItem(new GUIContent("Set (1, 1, 1)"), false, () =>
            {
                property.vector3Value = Vector3.one;
                property.serializedObject.ApplyModifiedProperties();
            });
        }
    }
}