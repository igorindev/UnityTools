using UnityEngine;

[CreateAssetMenu(fileName = "Localization Config ", menuName = "Localization/LocalizationConfig", order = 1)]
public class SOLocalizationConfig : ScriptableObject
{
    public string filePath = "Assets/Localization/LocalizationFile.csv";

#if UNITY_EDITOR
    public static string editorFilePath;
    void OnValidate()
    {
        editorFilePath = filePath;
        Debug.Log(editorFilePath + " setted as static.");
    }
#endif
}