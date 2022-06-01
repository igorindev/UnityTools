using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Localization
{
    [CustomEditor(typeof(LocalizationText))]
    public class CustomLocalizationText : Editor
    {
        SerializedProperty index;
        SerializedProperty context;
        public string text;

        void OnEnable()
        {
            index = serializedObject.FindProperty("index");
            context = serializedObject.FindProperty("contextIndex");
            text = index.intValue.ToString() + " - " + context.stringValue;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Identity", GUILayout.Width(200));
                
                if (EditorGUILayout.DropdownButton(new GUIContent(text), FocusType.Passive, EditorStyles.popup))
                {
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                                      new StringListSearchProvider(LocalizationManager.GetContexts(), (x) => { text = x; }));
                }
            }

            index.intValue = int.Parse(text.Split('-')[0]);
            string t = text.Split('-')[1];
            context.stringValue = t.Split(' ')[1];

            serializedObject.ApplyModifiedProperties();
        }
    }
}