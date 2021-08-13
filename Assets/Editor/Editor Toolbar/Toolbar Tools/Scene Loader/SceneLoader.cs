#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class SceneLoader : EditorWindow
{
    public static bool open = false;

    Vector2 scroll = Vector2.zero;
    static string sceneName;
    static string[] allScenes;
    static Texture2D back;
    public static SceneLoader window;

    static List<Scene> Scenes = new List<Scene>();

    public static void ShowWindow()
    {
        SceneGroup scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/Editor Toolbar/Toolbar Tools/Scene Loader/SceneGroup.asset") as SceneGroup;
        allScenes = scriptableObject.scenes;

        window = CreateInstance<SceneLoader>();

        Rect r = new Rect(UnityEditorWindow.GetEditorMainWindowPos());

        window.position = new Rect(r.x + r.width - 450, r.y + 29f, 240, 70 + Mathf.Clamp((32 * allScenes.Length), 0, 450));
        window.ShowPopup();

        Scenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scenes.Add(SceneManager.GetSceneAt(i));
        }
    }

    void OnLostFocus()
    {
        Close();
    }

    void OnGUI()
    {
        Texture tex = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Editor/Editor UI/PlusIcon.png", typeof(Texture));
        Texture texRemove = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Editor/Editor UI/RemoveIcon.png", typeof(Texture));

        back = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        back.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f));
        back.Apply();

        GUIStyle b = new GUIStyle(GUI.skin.textArea);
        b.normal.background = back;

        GUIStyle mystyle = new GUIStyle("Button")
        {
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
            padding = new RectOffset(0, 0, 0, 0)
        };

        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height), b);
        GUILayout.Space(8);
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(225));
        GUILayout.Space(13);

        GUILayout.Label("Scenes", EditorStyles.boldLabel);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d__Popup", "Edit Scenes"), mystyle, GUILayout.Width(20), GUILayout.Height(20)))
        {
            Selection.SetActiveObjectWithContext(AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/Editor Toolbar/Toolbar Tools/Scene Loader/SceneGroup.asset"), null);
            GetWindow(System.Type.GetType("UnityEditor.InspectorWindow, UnityEditor"));
        }

        GUILayout.EndHorizontal();

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
                GUILayout.BeginArea(new Rect(10, 60, 1000, 1000));
                scroll = EditorGUILayout.BeginScrollView(scroll, false, true, GUILayout.Width(Screen.width - 20), GUILayout.Height(Screen.height - 70));
                for (int i = 0; i < allScenes.Length; i++)
                {
                    sceneName = Path.GetFileNameWithoutExtension(allScenes[i]);

                    if (GUILayout.Button(i + " - " + sceneName, GUILayout.Width(130), GUILayout.Height(30)))
                    {
                        OpenScene(allScenes[i]);
                    }
                }

                GUILayout.BeginArea(new Rect(160, 2, 50, 1000));
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
                        if (GUILayout.Button(tex, GUILayout.Width(30), GUILayout.Height(30)))
                        {
                            OpenSceneAdd(allScenes[i]);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(texRemove, GUILayout.Width(30), GUILayout.Height(30)))
                        {
                            RemoveScene(id);
                        }
                    }

                }
                GUILayout.EndArea();

                EditorGUILayout.EndScrollView();
                GUILayout.EndArea();
            }
        }

        Texture2D ss = new Texture2D(1, 1);
        GUI.DrawTexture(new Rect(-1, 0, 242, 72 + Mathf.Clamp((32 * allScenes.Length), 0, 450)), ss, ScaleMode.StretchToFill, false, 0, new Color(0.33f, 0.33f, 0.33f, 1f), new Vector4(4, 0, 4, 4), 0);
        GUI.DrawTexture(new Rect(1, 1, 238, 68 + Mathf.Clamp((32 * allScenes.Length), 0, 450)), ss, ScaleMode.StretchToFill, true, 0, new Color(1, 1, 1, 0.7f), 1f, 4);
        GUI.EndGroup();
    }

    void OpenScene(string path)
    {
        if (EditorSceneManager.SaveOpenScenes())
        {
            Scenes = new List<Scene>();
            Scenes.Add(EditorSceneManager.OpenScene(path));
        }
    }
    void OpenSceneAdd(string path)
    {
        if (EditorSceneManager.SaveOpenScenes())
        {
            Scenes.Add(EditorSceneManager.OpenScene(path, OpenSceneMode.Additive));
        }
    }
    void RemoveScene(int id)
    {
        if (EditorSceneManager.SaveOpenScenes())
        {
            if (EditorSceneManager.CloseScene(Scenes[id], true))
            {
                Scenes.RemoveAt(id);
            }
        }
    }
}

public static class UnityEditorWindow
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
#endif