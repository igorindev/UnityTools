#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnityToolsWizard : EditorWindow
{
    List<GameObject> TraverseList = new List<GameObject>();

    Vector2 scroll;
    string projectPath = "Assets";

    UnityTools[] tools;

    [MenuItem("Tools/Unity Tools Wizard...", false, -10)]
    static void Init()
    {
        UnityToolsWizard sizeWindow = GetWindow<UnityToolsWizard>("Unity Tools Wizard");
        sizeWindow.autoRepaintOnSceneChange = true;
        sizeWindow.titleContent = new GUIContent("Unity Tools Wizard");
        sizeWindow.Show();
    }

    void OnEnable()
    {
    }

    void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Space(10);
            using (new EditorGUILayout.HorizontalScope(GUILayout.Height(20)))
            {
                GUILayout.Space(5);
                using (new EditorGUILayout.VerticalScope())
                {
                    projectPath = EditorGUILayout.TextField("Project Path", projectPath);
                    if (GUILayout.Button("Download", GUILayout.Width(100)))
                    {
                        Application.OpenURL("https://github.com/igorindev/UnityTools");
                    }
                }
                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    projectPath = EditorUtility.OpenFolderPanel("Unity Tools Project", "", "");
                    GUI.FocusControl("None");
                }
                GUILayout.Space(5);
            }
            using (var scrollScope = new EditorGUILayout.ScrollViewScope(scroll))
            {
                scroll = scrollScope.scrollPosition;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                for (int i = 0; i < 5; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Import@2x").image, GUILayout.Height(28), GUILayout.Width(28)))
                            {
                                ImportAsset("");
                            }
                            EditorGUILayout.LabelField("Scene Loader", EditorStyles.boldLabel, GUILayout.Height(28));

                            string description = Random.Range(0, 2) == 1 ? "Add a new window where you can configure this is ahah hah aha ha hha ha hah ah ahh ah ah  dd saas  w wedw  dsad sasd das ah" : "Add a new window where you can configure this";
                            float h = 15 * (int)(description.Length / 30f);
                            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel, GUILayout.Height(50));

                            GUILayout.FlexibleSpace();
                        }
                    }
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
            }
        }
    }

    void ImportAsset(string path)
    {
        string sourcePath = projectPath + "/" + path;
        try
        {
            FileUtil.CopyFileOrDirectory(sourcePath, path);
        }
        catch
        {
            Debug.LogError("This Tool is already on your project, or you have a folder with the same name and path as the import, not allowing a copy.");
        }
    }

    [System.Serializable]
    public struct UnityTools
    {
        public string toolName;
        public string toolPath;
        public string[] dependeciesPath;
    }
}
#endif