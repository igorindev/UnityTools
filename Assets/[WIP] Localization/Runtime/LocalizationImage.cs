using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Localization
{
    [DisallowMultipleComponent]
    public class LocalizationImage : Localization
    {
        [SerializeField] Sprite[] localizatedSprites;
        Image image;

        protected override void Initialize()
        {
            image = GetComponent<Image>();
        }

        public override void UpdateLocalization()
        { 
            int i = LocalizationManager.instance.SelectedLanguageIndex;
            if(i < localizatedSprites.Length)
            {
                image.sprite = localizatedSprites[i];
            }
            else
            {
                Debug.LogError("The language do not have an image setted, changing to first");
                image.sprite = localizatedSprites[0];
            }
            
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LocalizationImage))]
    public class CustomLocalizationImageInspector : Editor
    {
        SerializedProperty sp;
        string[] localization;
        string filePath = "Assets/[WIP] Localization/LocalizationFile.csv";

        void OnEnable()
        {
            sp = serializedObject.FindProperty("localizatedSprites");

            localization = LocalizationManager.GetLocalization(filePath);
            if (sp.arraySize != localization.Length)
            {                
                Sprite[] temp = new Sprite[sp.arraySize];
                for (int i = 0; i < sp.arraySize; i++)
                {
                    temp[i] = (Sprite)sp.GetArrayElementAtIndex(i).objectReferenceValue;
                }
                sp.arraySize = localization.Length;

                for (int i = 0; i < temp.Length; i++)
                {
                    sp.GetArrayElementAtIndex(i).objectReferenceValue = temp[i];
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

            for (int i = 0; i < localization.Length; i++)
            {
                SerializedProperty property = sp.GetArrayElementAtIndex(i);
                property.objectReferenceValue = EditorGUILayout.ObjectField(localization[i], property.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
            }

            EditorGUILayout.Space(10);

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
            if (GUILayout.Button("Update Localizations"))
            {
                localization = LocalizationManager.GetLocalization(filePath);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
