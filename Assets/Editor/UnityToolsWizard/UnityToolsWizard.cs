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
        tools = new UnityTools[]
        {
            new UnityTools("Colliders Explorer", "Editor/Colliders Explorer","Open a window where you can see all the colliders in the scene", new string[]{}),

            new UnityTools("Scene Loader", "Editor/SceneLoader", "Fast load scenes using the quick access window", new string[]
            {
                "Script Attributes"
            }),

            new UnityTools("Script Extension", "Script Extensions", "Extra reusable functions for calculations, debugging, fast use", new string[]{}),

            new UnityTools("Script Attributes", "Script Attributes", "Atrributes for code", new string[]{}),

            new UnityTools("Trigger Caller", "Trigger Caller", "Fast create primitive shaped trigger zones; define tags to collide, and assign functions to the OnEnter and OnExit UnityEvents.", new string[]{})
        };
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
                for (int i = 0; i < tools.Length; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Import@2x").image, GUILayout.Height(28), GUILayout.Width(28)))
                            {
                                ImportAsset("");
                            }
                            string Name = tools[i].toolName;

                            using (new EditorGUILayout.VerticalScope())
                            {
                                EditorGUILayout.LabelField(tools[i].toolName, EditorStyles.boldLabel, GUILayout.Height(28));
                                if (tools[i].dependeciesPath.Length > 0)
                                {
                                    EditorGUILayout.LabelField("Dependencies: ", GUILayout.Height(15));
                                    for (int j = 0; j < tools[i].dependeciesPath.Length; j++)
                                    {
                                        EditorGUILayout.LabelField("- " + tools[i].dependeciesPath[j], GUILayout.Height(15));
                                    }
                                }
                            }

                            string description = tools[i].toolDescription;
                            int m = Mathf.RoundToInt(description.Length / 25f);
                            float h = 30 + 8.5f * m;
                            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel, GUILayout.Height(h));
                            
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

        //check if need dependencie

        //ImportAsset()

    }

    [System.Serializable]
    public struct UnityTools
    {
        public UnityTools(string _toolName, string _toolPath, string _toolDescription, string[] _dependeciesPath)
        {
            toolName = _toolName;
            toolPath = _toolPath;
            toolDescription = _toolDescription;
            dependeciesPath = _dependeciesPath;
        }
        public string toolName;
        public string toolPath;
        public string toolDescription;
        public string[] dependeciesPath;
    }
}
#endif