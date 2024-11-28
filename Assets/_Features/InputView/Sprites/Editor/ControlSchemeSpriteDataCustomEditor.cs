using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Lamou.InputSystem.SpriteMap.EditorUtility
{
    [CustomPropertyDrawer(typeof(ControlSchemeSpriteData))]
    public class ControlSchemeSpriteDataCustomEditor : PropertyDrawer
    {
        private InputSpritesMap inputSpritesMap;
        private InputActionAsset actionMap;
        private ReadOnlyArray<InputControlScheme> schemes;

        public void Initialize(SerializedProperty property)
        {
            inputSpritesMap = property.serializedObject.targetObject as InputSpritesMap;
            actionMap = inputSpritesMap.inputActionAsset;
            schemes = actionMap.controlSchemes;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty DeviceLayoutName = property.FindPropertyRelative("DeviceLayoutName");
            SerializedProperty SpriteAtlas = property.FindPropertyRelative("SpriteAtlas");
            SerializedProperty GeneratedSpriteAsset = property.FindPropertyRelative("GeneratedSpriteAsset");
            SerializedProperty BindingsPreset = property.FindPropertyRelative("BindingsPreset");

            EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.BeginChangeCheck();

            rect.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUIUtility.wideMode = true;
            EditorGUI.indentLevel = 0;

            Initialize(property);

            List<string> schemaName = new List<string>();
            foreach (InputControlScheme item in schemes)
            {
                schemaName.Add(item.name);
            }

            int index = 0;
            for (int i = 0; i < schemes.Count; i++)
            {
                if (schemes[i].name == DeviceLayoutName.stringValue)
                {
                    index = i;
                    break;
                }
            }

            index = EditorGUI.Popup(rect, index, schemaName.ToArray());
            string result = schemaName[index];
            if (!string.IsNullOrEmpty(result))
            {
                DeviceLayoutName.stringValue = result;
            }

            EditorGUI.indentLevel = 1;

            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, SpriteAtlas);

            rect.y += EditorGUI.GetPropertyHeight(SpriteAtlas);
            EditorGUI.PropertyField(rect, GeneratedSpriteAsset);

            rect.y += EditorGUI.GetPropertyHeight(GeneratedSpriteAsset);
            EditorGUI.PropertyField(rect, BindingsPreset);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty DeviceLayoutName = property.FindPropertyRelative("DeviceLayoutName");
            SerializedProperty SpriteAtlas = property.FindPropertyRelative("SpriteAtlas");
            SerializedProperty GeneratedSpriteAsset = property.FindPropertyRelative("GeneratedSpriteAsset");
            SerializedProperty BindingsPreset = property.FindPropertyRelative("BindingsPreset");

            return EditorGUI.GetPropertyHeight(DeviceLayoutName)
            + EditorGUI.GetPropertyHeight(SpriteAtlas)
            + EditorGUI.GetPropertyHeight(GeneratedSpriteAsset)
            + EditorGUI.GetPropertyHeight(BindingsPreset);
        }
    }
}
