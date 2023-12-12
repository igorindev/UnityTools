using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ScriptEditorEmbedded : EditorWindow
{
    static bool scriptSelected;
    static MonoScript[] scripts;

    [SerializeField] MonoScript script;

    [SerializeField] string content = "";
    string backup = "";
    string currentPath = "";

    Vector2 scroll = new Vector2();

    GUIStyle styleA;
    GUIStyle styleB;

    [SerializeField] int id;

    [MenuItem("Assets/Edit Script...")]
    public static void Edit()
    {
        if (scripts == null || scripts.Length == 0)
        {
            scripts = Selection.GetFiltered<MonoScript>(SelectionMode.Assets);
        }
        Create();
    }
    static void Create()
    {
        ScriptEditorEmbedded window;
        if (HasOpenInstances<ScriptEditorEmbedded>())
        {
            window = CreateWindow<ScriptEditorEmbedded>(scripts[0].name + ".cs", GetWindow<ScriptEditorEmbedded>().GetType());
        }
        else
        {
            window = CreateWindow<ScriptEditorEmbedded>();
            window.position = new Rect(20, 80, 600, 600);
        }

        window.titleContent = new GUIContent(scripts[0].name + ".cs", EditorGUIUtility.IconContent("d_cs Script Icon").image);
    }

    [MenuItem("Assets/Edit Script...", true)]
    public static bool CheckIfScriptFile()
    {
        // enable the menu item only if a script file is selected
        // get only the scripts selected...
        scripts = Selection.GetFiltered<MonoScript>(SelectionMode.Assets);

        // ... but we only use the first
        scriptSelected = scripts.Length != 0 && scripts.Length < 2;

        return scriptSelected;
    }

    void OnEnable()
    {
        if (script == null)
        {
            scripts = Selection.GetFiltered<MonoScript>(SelectionMode.Assets);
            script = scripts[0];

            id = script.GetInstanceID();

            backup = content = ReadFile(AssetDatabase.GetAssetPath(id));

            styleA = new GUIStyle();
            styleA.normal.textColor = Color.white;
            styleA.contentOffset = new Vector2(7, 2);

            styleB = new GUIStyle("TextArea");
            styleB.wordWrap = false;
        }

        backup = content = ReadFile(AssetDatabase.GetAssetPath(id));
    }

    string ReadFile(string filePath)
    {
        currentPath = filePath;
        if (File.Exists(filePath))
        {
            StreamReader fileReader = new StreamReader(filePath, Encoding.GetEncoding("iso-8859-1"));
            string file = fileReader.ReadToEnd();

            fileReader.Close();

            return file;
        }
        else
        {
            Debug.LogError("File not found");
            return "File not found or is empty.";
        }
    }

    void SaveFile(string filePath)
    {
        string path = filePath;

        Debug.Log("Script saved at " + path);

        FileStream stream = new FileStream(path, FileMode.Create);

        StreamWriter fileWriter = new StreamWriter(stream, Encoding.GetEncoding("iso-8859-1"));

        fileWriter.Write(content);

        fileWriter.Close();

        AssetDatabase.ImportAsset(currentPath);
    }

    void OnGUI()
    {
        if (hasFocus)
        {
            if (Event.current != null && Event.current.isKey)
            {
                if (Event.current.control)
                {
                    if (Event.current.keyCode == KeyCode.Z && Event.current.type == EventType.KeyDown)
                    {
                        UndoCmd();
                        Event.current.Use();
                    }
                    if (Event.current.keyCode == KeyCode.Y && Event.current.type == EventType.KeyDown)
                    {
                        RedoCmd();
                        Event.current.Use();
                    }
                }                
            }
        }

        string[] lines = content.Split('\n');

        int d = GetDigits(lines.Length - 1, 0);
        d *= 7;
        d += 10;

        EditorGUILayout.BeginVertical(GUILayout.Height(Screen.height - 20));

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh@2x", "Restore"), GUILayout.Height(20)))
        {
            Restore();
        }
        if (GUILayout.Button(EditorGUIUtility.IconContent("SaveAs@2x", "Save"), GUILayout.Height(20)))
        {
            SaveFile(currentPath);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(Screen.height - 45));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.MinWidth(d), GUILayout.MaxWidth(d));

        for (int i = 0; i < lines.Length; i++)
        {
            EditorGUILayout.LabelField(i.ToString(), styleA, GUILayout.Height(13), GUILayout.MaxWidth(d));
        }
        EditorGUILayout.LabelField("", styleA, GUILayout.Height(15), GUILayout.MaxWidth(d));

        EditorGUILayout.EndVertical();

        EditorGUI.BeginChangeCheck();
        string c = GUILayout.TextArea(content, styleB);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Modify Script in ScriptEditorEmbedded");
            content = c;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    void UndoCmd()
    {
        Undo.PerformUndo();
    }
    void RedoCmd()
    {
        Undo.PerformRedo();
    }

    void Restore()
    {
        content = backup;
    }

    static int GetDigits(int n1, int numDigits)
    {
        if (n1 == 0)
            return numDigits;

        return GetDigits(n1 / 10, ++numDigits);
    }
}
