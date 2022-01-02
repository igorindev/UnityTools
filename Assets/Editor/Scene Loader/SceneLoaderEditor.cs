#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.Toolbars;
using UnityEditor.Overlays;

namespace SceneLoader
{
    [InitializeOnLoad, ExecuteInEditMode]
    public class SceneLoaderEditor : EditorWindow
    {
        public static bool open = false;

        Vector2 scroll = Vector2.zero;
        static string sceneName;
        static string[] allScenes;
        static Texture2D back;
        public static SceneLoaderEditor window;

        static List<Scene> Scenes = new List<Scene>();

        static Texture plus, remove;
        static Rect SceneViewPosition;

#if !UNITY_2021_2_OR_NEWER
        static SceneLoaderEditor()
        {
            SceneView.duringSceneGui += OnScene;
        }

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
        public static void ShowWindow()
        {
            SceneGroup scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/Scene Loader/SceneGroup.asset") as SceneGroup;
            allScenes = scriptableObject.scenes;

            window = CreateInstance<SceneLoaderEditor>();

            SceneViewPosition = GetWindow<SceneView>().position;

            window.position = new Rect(SceneViewPosition.x, SceneViewPosition.y + 45f, 240, Mathf.Clamp((32 * allScenes.Length), 0, SceneViewPosition.height - 26));
            window.ShowPopup();

            Scenes = new List<Scene>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scenes.Add(SceneManager.GetSceneAt(i));
            }
        }

        void OnEnable()
        {
            plus = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Editor/Scene Loader/UI/PlusIcon.png", typeof(Texture));
            remove = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Editor/Scene Loader/UI/RemoveIcon.png", typeof(Texture));

            back = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            back.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f));
            back.Apply();
        }

        void OnLostFocus()
        {
            Close();
        }

        void OnGUI()
        {
            GUIStyle b = new GUIStyle(GUI.skin.textArea);
            b.normal.background = back;

            GUIStyle mystyle = new GUIStyle("Button")
            {
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                padding = new RectOffset(0, 0, 0, 0)
            };

            using (new GUI.GroupScope(new Rect(0, 0, Screen.width, Screen.height), b))
            {
                GUILayout.Space(8);
                using (new GUILayout.HorizontalScope(GUILayout.MaxWidth(225)))
                {
                    GUILayout.Space(13);

                    GUILayout.Label("Scenes", EditorStyles.boldLabel);

                    if (GUILayout.Button(EditorGUIUtility.IconContent("d__Popup", "Edit Scenes"), mystyle, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        Selection.SetActiveObjectWithContext(AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/Scene Loader/SceneGroup.asset"), null);
                        GetWindow(System.Type.GetType("UnityEditor.InspectorWindow, UnityEditor"));
                    }
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close", "Edit Scenes"), mystyle, GUILayout.Width(20), GUILayout.Height(20)))
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
                        using (new GUILayout.AreaScope(new Rect(10, 60, 1000, 1000)))
                        {
                            using (var ScrollView = new EditorGUILayout.ScrollViewScope(scroll, false, true, GUILayout.Width(Screen.width - 20), GUILayout.Height(Screen.height - 70)))
                            {
                                scroll = ScrollView.scrollPosition;

                                for (int i = 0; i < allScenes.Length; i++)
                                {
                                    sceneName = Path.GetFileNameWithoutExtension(allScenes[i]);

                                    if (GUILayout.Button(sceneName, GUILayout.Width(130), GUILayout.Height(30)))
                                    {
                                        OpenScene(allScenes[i]);
                                    }
                                }

                                using (new GUILayout.AreaScope(new Rect(160, 2, 50, 1000)))
                                {
                                    for (int i = 0; i < allScenes.Length; i++)
                                    {
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
                                            if (GUILayout.Button(plus, GUILayout.Width(30), GUILayout.Height(30)))
                                            {
                                                OpenSceneAdd(allScenes[i]);
                                            }
                                        }
                                        else
                                        {
                                            if (GUILayout.Button(remove, GUILayout.Width(30), GUILayout.Height(30)))
                                            {
                                                RemoveScene(id);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                Texture2D ss = new Texture2D(1, 1);

                GUI.DrawTexture(new Rect(-1, 0, 242, Mathf.Clamp((32 * allScenes.Length), 0, window.position.height + 2)), ss, ScaleMode.StretchToFill, false, 0, new Color(0.33f, 0.33f, 0.33f, 1f), new Vector4(4, 0, 4, 4), 0); ;
                GUI.DrawTexture(new Rect(1, 1, 238, Mathf.Clamp((32 * allScenes.Length), 0, window.position.height - 2)), ss, ScaleMode.StretchToFill, true, 0, new Color(1, 1, 1, 0.7f), 1f, 4);
            }
        }

        void OpenScene(string path)
        {
            bool noUntitledSceneFound = EditorSceneManager.EnsureUntitledSceneHasBeenSaved("The Current Scene is Untitled");

            if (EditorSceneManager.SaveOpenScenes() || noUntitledSceneFound)
            {
                Scenes = new List<Scene>();
                Scenes.Add(EditorSceneManager.OpenScene(path));
            }
        }
        void OpenSceneAdd(string path)
        {
            bool noUntitledSceneFound = EditorSceneManager.EnsureUntitledSceneHasBeenSaved("The Current Scene is Untitled");

            if (EditorSceneManager.SaveOpenScenes() || noUntitledSceneFound)
            {
                Scenes.Add(EditorSceneManager.OpenScene(path, OpenSceneMode.Additive));
            }
        }
        void RemoveScene(int id)
        {
            bool noUntitledSceneFound = EditorSceneManager.EnsureUntitledSceneHasBeenSaved("The Current Scene is Untitled");

            if (EditorSceneManager.SaveOpenScenes() || noUntitledSceneFound)
            {
                if (EditorSceneManager.CloseScene(Scenes[id], true))
                {
                    Scenes.RemoveAt(id);
                }
            }
        }
    }
    public static class SceneLoaderFunctions
    {
        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }
            return result.ToArray();
        }

        public static Rect GetEditorMainWindowPos()
        {
            var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
            if (containerWinType == null)
                throw new System.MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (showModeField == null || positionProperty == null)
                throw new System.MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            var windows = Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (var win in windows)
            {
                var showmode = (int)showModeField.GetValue(win);
                if (showmode == 4) // main window
                {
                    var pos = (Rect)positionProperty.GetValue(win, null);
                    return pos;
                }
            }
            throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
        }

        public static void CenterOnMainWin(this UnityEditor.EditorWindow aWin)
        {
            var main = GetEditorMainWindowPos();
            var pos = aWin.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            aWin.position = pos;
        }
    }

#if UNITY_2021_2_OR_NEWER
    [EditorToolbarElement(id, typeof(SceneView))]
    class SceneLoaderOverlay : EditorToolbarButton, IAccessContainerWindow
    {
        public const string id = "ToolBar/SceneLoader";
        public EditorWindow containerWindow { get; set; }

        public SceneLoaderOverlay()
        {
            text = "";
            icon = EditorGUIUtility.IconContent("winbtn_win_restore_h", "").image as Texture2D;

            tooltip = "Open a menu that shows all assigned scenes, allowing to load then.";
            clicked += OnClick;
        }

        void OnClick()
        {
            SceneLoaderEditor.ShowWindow();
        }
    }

    [Overlay(typeof(SceneView), "Scene Loader")]
    public class SceneLoaderToolBar : ToolbarOverlay
    {
        SceneLoaderToolBar() : base(SceneLoaderOverlay.id) { }
    }
}
#endif
#endif