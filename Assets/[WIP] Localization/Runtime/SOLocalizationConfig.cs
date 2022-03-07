using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Localization Config ", menuName = "Localization/LocalizationConfig", order = 1)]
public class SOLocalizationConfig : ScriptableObject
{
    [SerializeField] string path = "Assets/Localization/LocalizationFile.csv";
    public static string filePath;

    void OnValidate()
    {
        filePath = path;
    }
    [ContextMenu("Test")]
    void Test()
    {
        Debug.Log(filePath);
    }
}

[CustomEditor(typeof(SOLocalizationConfig))]
public class CustomSOLocalizationConfigInspector : Editor
{
    SerializedProperty path;

    void OnEnable()
    {
        path = serializedObject.FindProperty("path");
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

