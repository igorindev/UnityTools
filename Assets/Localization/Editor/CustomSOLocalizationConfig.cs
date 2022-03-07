using UnityEngine;
using UnityEditor;

namespace Localization 
{
    [CustomEditor(typeof(SOLocalizationConfig))]
    public class CustomSOLocalizationConfig : Editor
    {
        SerializedProperty path;

        void OnEnable()
        {
            path = serializedObject.FindProperty("filePath");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUILayout.HorizontalScope(GUILayout.Height(20)))
            {
                EditorGUILayout.PropertyField(path);

                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    string temp = EditorUtility.OpenFilePanel("Unity Tools Project", "", "csv");
                    if (string.IsNullOrEmpty(temp) == false)
                    {
                        path.stringValue = temp;
                    }
                }
            }
            if (serializedObject.hasModifiedProperties)
            {
                GUI.FocusControl("None");
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
