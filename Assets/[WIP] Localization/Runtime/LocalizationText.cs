using TMPro;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace Localization
{
    [DisallowMultipleComponent]
    public class LocalizationText : Localization
    {
        [SerializeField] int index = 1;
        [SerializeField] string contextIndex = "context_name_value";

        TMP_Text text;

        protected override void Initialize()
        {
            text = GetComponent<TMP_Text>();
        }

        public override void UpdateLocalization()
        {
            text.text = LocalizationManager.instance.GetContentAtIndex(index, gameObject);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LocalizationText))]
    public class CustomLocalizationTextInspector : Editor
    {
        SerializedProperty index;
        SerializedProperty context;
        public string text;
        string filePath = "Assets/[WIP] Localization/LocalizationFile.csv";

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

            using (new EditorGUILayout.HorizontalScope(GUILayout.Height(20)))
            {
                filePath = EditorGUILayout.TextField("File Path", filePath);

                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    string temp = EditorUtility.OpenFilePanel("Unity Tools Project", "", "csv");
                    if (string.IsNullOrEmpty(temp) == false)
                    {
                        filePath = temp;
                    }
                    GUI.FocusControl("None");
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Text Context", GUILayout.Width(200));
                
                if (GUILayout.Button(text, EditorStyles.popup))
                {
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                                      new StringListSearchProvider(LocalizationManager.GetContexts(filePath), (x) => { text = x; }));
                }
            }

            index.intValue = int.Parse(text.Split('-')[0]);
            string t = text.Split('-')[1];
            context.stringValue = t.Split(' ')[1];

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}