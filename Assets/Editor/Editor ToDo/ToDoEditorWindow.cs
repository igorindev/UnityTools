using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ToDoEditorWindow : EditorWindow
{
    static EditorWindow window;
    static Tasks tasksObj;
    static bool[] completed;

    Vector2 scroll;

    public static void ShowWindow()
    {
        tasksObj = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/Editor ToDo/Tasks.asset") as Tasks;

        window = GetWindow<ToDoEditorWindow>();

        window.Show();

        completed = new bool[tasksObj.tasks.Count];
        for (int i = 0; i < completed.Length; i++)
        {
            completed[i] = tasksObj.tasks[i].completed;
        }
    }

    void OnGUI()
    {
        Texture2D back = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        back.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f));
        back.Apply();
        Texture2D front = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        front.SetPixel(0, 0, new Color(0.24f, 0.24f, 0.24f));
        front.Apply();
        Texture2D a = new Texture2D(1, 1, TextureFormat.RGBA32, false);

        GUIStyle d = EditorStyles.helpBox;
        d.normal.background = front;

        GUIStyle b = new GUIStyle();
        b.normal.background = back;

        GUIStyle c = new GUIStyle();
        b.normal.background = back;

        GUIStyle mystyle = new GUIStyle("Button")
        {
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
            padding = new RectOffset(0, 0, 0, 0)
        };

        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height), b);

        GUILayout.Space(8);
        GUILayout.BeginHorizontal();

        GUILayout.Space(13);
        GUILayout.Label("To Do", EditorStyles.boldLabel);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d__Popup", "Edit Scenes"), mystyle, GUILayout.Width(20), GUILayout.Height(20)))
        {
            Selection.SetActiveObjectWithContext(AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/Editor ToDo/Tasks.asset"), null);
            GetWindow(System.Type.GetType("UnityEditor.InspectorWindow, UnityEditor"));
        }

        GUILayout.EndHorizontal();

        scroll = EditorGUILayout.BeginScrollView(scroll, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(Screen.width-5));
        GUILayout.BeginVertical();

        for (int i = 0; i < tasksObj.tasks.Count; i++)
        {
            GUI.enabled = true;
            GUILayout.BeginVertical(d);

            switch (tasksObj.tasks[i].priority)
            {
                case Task.Priority.High:
                    a.SetPixel(0, 0, Color.red);
                    break;
                case Task.Priority.Medium:
                    a.SetPixel(0, 0, Color.yellow);
                    break;
                case Task.Priority.Low:
                    a.SetPixel(0, 0, Color.green);
                    break;
            }

            a.Apply();
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = a;

            GUILayout.BeginHorizontal();
            GUILayout.Box(i.ToString(), boxStyle, GUILayout.Width(20), GUILayout.Height(20));

            GUILayout.BeginVertical();
            GUI.enabled = !completed[i];
            tasksObj.tasks[i].author = GUILayout.TextField(tasksObj.tasks[i].author);
            tasksObj.tasks[i].task = GUILayout.TextArea(tasksObj.tasks[i].task, GUILayout.ExpandHeight(true));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            completed[i] = GUILayout.Toggle(completed[i], "Completed");
            tasksObj.tasks[i].completed = completed[i];

            GUILayout.Label("", EditorStyles.boldLabel);

            GUI.enabled = !string.IsNullOrEmpty(tasksObj.tasks[i].onScene);

            if (GUILayout.Button("Open Scene"))
            {
                OpenScene(tasksObj.tasks[i].onScene);
            }
            GUI.enabled = true;
            if (GUILayout.Button("Remove"))
            {
                RemoveTask(i);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        GUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        GUI.enabled = true;
        if (GUILayout.Button("Add task"))
        {
            AddTask();
        }

        GUI.EndGroup();
    }

    void OpenScene(string path)
    {
        if (EditorSceneManager.SaveOpenScenes())
        {
            EditorSceneManager.OpenScene(path);
        }
    }
    void AddTask()
    {
        tasksObj.tasks.Add(new Task());
        completed = new bool[tasksObj.tasks.Count];
        for (int i = 0; i < completed.Length; i++)
        {
            completed[i] = tasksObj.tasks[i].completed;
        }
    }
    void RemoveTask(int id)
    {
        tasksObj.tasks.RemoveAt(id);
        completed = new bool[tasksObj.tasks.Count];
        for (int i = 0; i < completed.Length; i++)
        {
            completed[i] = tasksObj.tasks[i].completed;
        }
    }
}
