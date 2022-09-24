using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Localization
{
    [CustomEditor(typeof(LocalizationText))]
    [CanEditMultipleObjects]
    public class CustomLocalizationText : Editor
    {
        SerializedProperty index;
        SerializedProperty context;
        public string text;

        public static StringListSearchProvider inst;

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

            GUILayout.Space(8);
            GUILayout.Label("Identity", GUILayout.Width(200));

            using (new EditorGUILayout.HorizontalScope())
            {
                if (EditorGUILayout.DropdownButton(new GUIContent(text), FocusType.Passive, EditorStyles.popup))
                {
                    if (inst == null)
                        inst = CreateInstance(nameof(StringListSearchProvider)) as StringListSearchProvider;

                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                                     inst.Initialize(LocalizationManager.GetContexts(), (x) => { text = x; }));
                }
            }

            index.intValue = int.Parse(text.Split('-')[0]);
            string t = text.Split('-')[1];
            context.stringValue = t.Split(' ')[1];

            serializedObject.ApplyModifiedProperties();
        }
    }
}