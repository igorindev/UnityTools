using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    public class SceneLoaderEditor : EditorWindow
    {
        static Texture plusTexture;
        static Texture minusTexture;

        static Vector2 scroll = Vector2.zero;
        static string sceneName;
        static string[] allScenes;

        static SceneLoaderEditor window;

        static Texture2D backTexture;

        static List<Scene> Scenes = new List<Scene>();

        static Rect SceneViewPosition;

        static SceneLoaderGroup scriptableObject;

        public const string uiPath = "Packages/com.lamou.sceneloader/Editor/UI";
        public const string folderFixed = "Assets/Editor/SceneLoaderGroup.asset";

#if !UNITY_2021_2_OR_NEWER
        private static void OnScene(SceneView sceneview)
        {
            GUIStyle mystyle = new GUIStyle("Button")
            {
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                padding = new RectOffset(0, 0, 0, 0)
            };
            Handles.BeginGUI();
            if (GUI.Button(new Rect(6, 6, 57, 20), "Scenes", mystyle))
                ShowWindow();

            Handles.EndGUI();
        }
#endif
        void OnEnable()
        {
#if !UNITY_2021_2_OR_NEWER
            SceneView.duringSceneGui += OnScene;
#endif
            minusTexture = AssetDatabase.LoadAssetAtPath($"{uiPath}/MinusIcon.png", typeof(Texture)) as Texture;
            plusTexture = AssetDatabase.LoadAssetAtPath($"{uiPath}/PlusIcon.png", typeof(Texture)) as Texture;

            backTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            backTexture.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f));
            backTexture.Apply();
        }

        void OnDisable()
        {
#if !UNITY_2021_2_OR_NEWER
            SceneView.duringSceneGui -= OnScene;
#endif
        }

        void OnLostFocus()
        {
            Close();
        }

        void OnGUI()
        {
            GUIStyle textSkin = new GUIStyle(GUI.skin.textArea);
            textSkin.normal.background = backTexture;

            GUIStyle myStyle = new GUIStyle("Button")
            {
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                padding = new RectOffset(0, 0, 0, 0)
            };

            using (new GUI.GroupScope(new Rect(0, 0, Screen.width, Screen.height), textSkin))
            {
                GUILayout.Space(8);
                using (new GUILayout.HorizontalScope(GUILayout.MaxWidth(225)))
                {
                    GUILayout.Space(13);

                    GUILayout.Label("Scenes", EditorStyles.boldLabel);

                    if (GUILayout.Button(EditorGUIUtility.IconContent("d__Popup", "Edit Scenes"), myStyle, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        Selection.SetActiveObjectWithContext(AssetDatabase.LoadAssetAtPath<ScriptableObject>(folderFixed), null);
                        GetWindow(System.Type.GetType("UnityEditor.InspectorWindow, UnityEditor"));
                    }
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close", "Edit Scenes"), myStyle, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        Close();
                    }
                }

                GUI.Label(new Rect(15, 40, 80, 20), "Single", EditorStyles.boldLabel);
                GUI.Label(new Rect(170, 40, 80, 20), "Additive", EditorStyles.boldLabel);

                if (Application.isPlaying)
                {
                    GUI.Label(new Rect(15, 70, 1000, 1000), "Disabled during PlayMode", EditorStyles.largeLabel);
                }
                else
                {
                    if (allScenes.Length > 0)
                    {
                        {
                            using GUI.ScrollViewScope ScrollView = new GUI.ScrollViewScope(new Rect(10, 65, 220, Screen.height - 70), scroll, new Rect(0, 32.5f, 0, allScenes.Length * 32.5f));
                            scroll = ScrollView.scrollPosition;

                            for (int i = 0; i < allScenes.Length; i++)
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    sceneName = Path.GetFileNameWithoutExtension(allScenes[i]);

                                    if (GUILayout.Button(sceneName, GUILayout.Width(130), GUILayout.Height(30)))
                                    {
                                        OpenScene(allScenes[i]);
                                    }

                                    GUILayout.Space(20);

                                    bool isLoaded = false;
                                    int id = 0;
                                    sceneName = Path.GetFileNameWithoutExtension(allScenes[i]);
                                    for (int j = 0; j < Scenes.Count; j++)
                                    {
                                        if (Scenes[j].name == sceneName)
                                        {
                                            isLoaded = true;
                                            id = j;
                                        }
                                    }

                                    if (!isLoaded)
                                    {
                                        if (GUILayout.Button(plusTexture, GUILayout.Width(30), GUILayout.Height(30)))
                                        {
                                            OpenSceneAdd(allScenes[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (GUILayout.Button(minusTexture, GUILayout.Width(30), GUILayout.Height(30)))
                                        {
                                            RemoveScene(id);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Texture2D ss = new Texture2D(1, 1);

                GUI.DrawTexture(new Rect(-1, 0, 242, window.position.height + 2), ss, ScaleMode.StretchToFill, false, 0, new Color(0.33f, 0.33f, 0.33f, 1f), new Vector4(4, 0, 4, 4), 0); ;
                GUI.DrawTexture(new Rect(1, 1, 238, window.position.height - 2), ss, ScaleMode.StretchToFill, true, 0, new Color(1, 1, 1, 0.7f), 1f, 4);
            }
        }

        public static void ShowWindow()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
            {
                AssetDatabase.CreateFolder("Assets", "Editor");
            }
            
            if (scriptableObject == null)
            {
                scriptableObject = AssetDatabase.LoadAssetAtPath(folderFixed, typeof(SceneLoaderGroup)) as SceneLoaderGroup;
                if (scriptableObject == null)
                {
                    SceneLoaderGroup sceneLoaderGroup = (SceneLoaderGroup)CreateInstance(typeof(SceneLoaderGroup));
                    AssetDatabase.CreateAsset(sceneLoaderGroup, folderFixed);
                    AssetDatabase.SaveAssets();

                    scriptableObject = sceneLoaderGroup;
                }
            }

            allScenes = scriptableObject.scenes ?? (new string[0] { });

            window = CreateInstance<SceneLoaderEditor>();

            SceneViewPosition = GetWindow<SceneView>().position;
#if UNITY_2021_2_OR_NEWER
            Vector2 v = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            v.x -= 35;
            v.y -= 85;
#else
            Vector2 v = new Vector2(SceneViewPosition.x, SceneViewPosition.y);
#endif
            window.position = new Rect(v.x, v.y, 240, Mathf.Clamp(32 * allScenes.Length, SceneViewPosition.height - 22, SceneViewPosition.height - 22));
            window.ShowPopup();

            Scenes = new List<Scene>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scenes.Add(SceneManager.GetSceneAt(i));
            }
        }

        void OpenScene(string path)
        {
            if (EditorSceneManager.EnsureUntitledSceneHasBeenSaved("The Current Scene is Untitled"))
            {
                if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(Scenes.ToArray()))
                {
                    Scenes = new List<Scene>
                    {
                        EditorSceneManager.OpenScene(path)
                    };
                }
            }
            else
            {
                if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(Scenes.ToArray()))
                {
                    Scenes = new List<Scene>
                    {
                        EditorSceneManager.OpenScene(path)
                    };
                }
            }
        }

        void OpenSceneAdd(string path)
        {
            Scenes.Add(EditorSceneManager.OpenScene(path, OpenSceneMode.Additive));
        }

        void RemoveScene(int id)
        {
            bool saveUntitledScene = EditorSceneManager.EnsureUntitledSceneHasBeenSaved("The current Scene is Untitled and the changes are not saved. Save it?"); 

            if (saveUntitledScene) { }

            if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[1] { Scenes[id] }))
            {
                if (EditorSceneManager.CloseScene(Scenes[id], true))
                {
                    Scenes.RemoveAt(id);
                }
            }
        }
    }
}