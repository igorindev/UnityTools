using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class LocalizationEditor : EditorWindow
{
    [SerializeField] string filePath = "Assets/Localization/LocalizationFile.csv";
    [SerializeField] bool replaceFile = false;

    int numOfLines = 0;
    string[,] texts = new string[0, 0];
    string[] languagesFoundInFile = new string[0];
    string fileContent;

    bool[] selected;

    const char splitValue = ';';

    Vector2 scroll;
    Vector2[,] scrollArea;

    [MenuItem("Tools/Localization/Localization Editor...")]
    static void Create()
    {
        var window = GetWindow<LocalizationEditor>("Localization Editor");
        window.position = new Rect(0, 0, 900, 600);
        window.Show();
    }

    void OnEnable()
    {
        ReadLanguageFile();
    }

    void ReadLanguageFile()
    {
        ReadContent();
        string[] lines = fileContent.Replace("\r", "").Split('\n');
        numOfLines = lines.Length - 1;
        selected = new bool[numOfLines];
        texts = new string[numOfLines, languagesFoundInFile.Length];
        scrollArea = new Vector2[numOfLines, languagesFoundInFile.Length];
        LoadTexts();
    }

    void Reload()
    {
        texts = new string[numOfLines, languagesFoundInFile.Length];
        scrollArea = new Vector2[numOfLines, languagesFoundInFile.Length];
    }

    void ModifyLines(int add = 1)
    {
        numOfLines += add;
        if (numOfLines < 2)
        {
            numOfLines = 2;
            return;
        }

        string[,] temp = texts;

        Reload();

        for (int l = 0; l < texts.GetLength(0); l++)
        {
            for (int c = 0; c < texts.GetLength(1); c++)
            {
                if (add > 0)
                {
                    if (l < numOfLines - 1)
                    {
                        texts[l, c] = temp[l, c];
                    }
                    else
                    {
                        texts[l, c] = "-";
                    }
                }
                else
                {
                    if (l < numOfLines)
                    {
                        texts[l, c] = temp[l, c];
                    }
                    else
                    {
                        texts[l, c] = "-";
                    }
                }
            }
        }
    }
    void ModifyLanguage(int add = 1)
    {
        string[] t = languagesFoundInFile;

        int toAdd = t.Length + add;
        toAdd = toAdd < 2 ? 2 : toAdd;

        languagesFoundInFile = new string[toAdd];

        for (int i = 0; i < languagesFoundInFile.Length; i++)
        {
            if (i < t.Length)
            {
                languagesFoundInFile[i] = t[i];
            }
            else
            {
                languagesFoundInFile[i] = "New Language";
            }
        }

        string[,] temp = texts;

        Reload();

        for (int l = 0; l < texts.GetLength(0); l++)
        {
            for (int c = 0; c < texts.GetLength(1); c++)
            {
                if (c < t.Length)
                {
                    texts[l, c] = temp[l, c];
                }
                else
                {
                    texts[l, c] = "-";
                }
            }
        }
    }

    void SaveTexts()
    {
        SaveTextFile();
    }
    void LoadTexts()
    {
        for (int l = 0; l < languagesFoundInFile.Length; l++)
        {
            for (int i = 1; i < numOfLines; i++)
            {
                texts[i, l] = GetContentAtIndex(i, l);
            }
        }
    }

    void ReadContent()
    {
        fileContent = Read();
        ReadPreSavedFile();
    }
    string Read()
    {
        if (File.Exists(filePath))
        {
            StreamReader fileReader = new StreamReader(filePath, Encoding.GetEncoding("iso-8859-1"));
            string file = fileReader.ReadToEnd();

            fileReader.Close();

            return file;
        }
        else
        {
            Debug.LogError("Localization file not found");
            return "File not found or is empty.";
        }
    }

    void ReadPreSavedFile()
    {
        if (fileContent != "")
        {
            languagesFoundInFile = GetLine(1).Split(splitValue);
            languagesFoundInFile[0] = "(Fake Position - Ignore)";
        }
        else
        {
            Debug.LogError("No pre saved file");
        }
    }
    string GetContentAtIndex(int lineNo, int colum)
    {
        string[] lines = fileContent.Replace("\r", "").Split('\n');
        string temp = lines.Length >= lineNo ? lines[lineNo] : null;

        string result = temp.Split(splitValue)[colum];

        return result;
    }
    string GetLine(int lineNo)
    {
        string[] lines = fileContent.Replace("\r", "").Split('\n');
        return lines.Length >= lineNo ? lines[lineNo - 1] : null;
    }

    void SaveTextFile()
    {
        if (replaceFile)
        {
            string path = filePath;

            string file = CreateFileContent();

            Debug.Log("Language file replaced at " + path);

            FileStream stream = new FileStream(path, FileMode.Create);

            StreamWriter fileWriter = new StreamWriter(stream, Encoding.GetEncoding("iso-8859-1"));

            fileWriter.Write(file);

            fileWriter.Close();
        }
        else
        {
            string path = filePath;
            while (File.Exists(path))
            {
                string temp = path.Split('.')[0];
                path = temp + "New.cvs";
            }

            string file = CreateFileContent();

            Debug.Log("New Language File Created at " + path);

            FileStream stream = new FileStream(path, FileMode.CreateNew);

            StreamWriter fileWriter = new StreamWriter(stream, Encoding.GetEncoding("iso-8859-1"));

            fileWriter.Write(file);

            fileWriter.Close();
        }
    }
    string CreateFileContent()
    {
        string language = "";
        string line = "";
        for (int i = 0; i < languagesFoundInFile.Length; i++)
        {
            if (i < languagesFoundInFile.Length - 1)
            {
                language += languagesFoundInFile[i] + splitValue;
            }
            else
            {
                language += languagesFoundInFile[i];
            }
        }

        for (int l = 1; l < texts.GetLength(0); l++)
        {
            for (int c = 0; c < texts.GetLength(1); c++)
            {
                if (c < texts.GetLength(1) - 1)
                {
                    line += texts[l, c] + splitValue;
                }
                else
                {
                    line += texts[l, c];
                }
            }
            line += "\n";
        }

        return language + "\n" + line;
    }

    void CopyLine()
    {

    }

    void PasteLine()
    {

    }

    static void DrawHorizontalUILine(Color color, float size = 6, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += size;
        EditorGUI.DrawRect(r, color);
    }

    void OnGUI()
    {
        if (EditorApplication.isPlaying)
        {
            GUILayout.Label("Disabled during PlayMode", EditorStyles.largeLabel);
            return;
        }

        GUILayout.Label("");

        GUI.BeginGroup(new Rect(20, 10, Screen.width, Screen.height));
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(500), GUILayout.Height(15)))
        {
            filePath = EditorGUILayout.TextField("File Path", filePath);
            replaceFile = EditorGUILayout.Toggle("Replace Current File", replaceFile);
        }

        GUILayout.Label("");

        using (new EditorGUILayout.HorizontalScope(GUILayout.Width(100), GUILayout.Height(15)))
        {
            if (GUILayout.Button("Add new Language", GUILayout.Width(130), GUILayout.Height(22)))
            {
                ModifyLanguage();
            }
            if (languagesFoundInFile.Length <= 2) { GUI.enabled = false; }
            if (GUILayout.Button("Remove Language", GUILayout.Width(130), GUILayout.Height(22)))
            {
                ModifyLanguage(-1);
            }
            GUI.enabled = true;
            if (GUILayout.Button("Add new Line", GUILayout.Width(130), GUILayout.Height(22)))
            {
                ModifyLines();
            }
            if (numOfLines <= 2) { GUI.enabled = false; }
            if (GUILayout.Button("Remove Line", GUILayout.Width(130), GUILayout.Height(22)))
            {
                ModifyLines(-1);
            }
            GUI.enabled = true;
            if (GUILayout.Button("Save", GUILayout.Width(130), GUILayout.Height(22)))
            {
                SaveTexts();
            }
            if (GUILayout.Button("Reload", GUILayout.Width(130), GUILayout.Height(22)))
            {
                ReadLanguageFile();
            }
        }

        EditorGUILayout.Space(10);
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(800)))
        {
            DrawHorizontalUILine(Color.gray, 6, 1, 0);
        }

        EditorGUILayout.Space(10);

        GUIStyle style = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(0, 0, 5, 0),
            fontSize = 12
        };

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 35), GUILayout.Height(Screen.height - 180));

        EditorGUILayout.BeginHorizontal(style, GUILayout.Width(240));
        EditorGUILayout.BeginVertical(GUILayout.Width(240), GUILayout.Height(1));
        EditorGUILayout.LabelField(" ID      Context Name       |       Languages:");
        EditorGUILayout.Space(2);
        DrawHorizontalUILine(Color.gray, 6, 1, 0);

        EditorGUILayout.EndVertical();

        for (int l = 1; l < languagesFoundInFile.Length; l++)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(220), GUILayout.Height(10));

            languagesFoundInFile[l] = EditorGUILayout.TextField(languagesFoundInFile[l]);
            EditorGUILayout.Space(2);
            DrawHorizontalUILine(Color.gray, 6, 1, 0);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        for (int l = 0; l < languagesFoundInFile.Length; l++)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(220), GUILayout.Height(100));

            for (int i = 1; i < numOfLines; i++)
            {
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                if (l == 0)
                {
                    GUIStyle n = new GUIStyle
                    {
                        padding = new RectOffset(5, 0, 40, 0),
                        fontStyle = FontStyle.Bold,
                        fontSize = 12,
                    };
                    n.normal.textColor = Color.white;

                    EditorGUILayout.LabelField(i.ToString(), n, GUILayout.Width(30));

                    EditorGUILayout.BeginVertical();
                    DrawHorizontalUILine(new Vector4(0, 0.7f, 0, 1), 1, 3, 0);
                    texts[i, l] = GUILayout.TextArea(texts[i, l], GUILayout.Width(180), GUILayout.Height(100));
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.BeginVertical();
                    DrawHorizontalUILine(new Vector4(0, 0.7f, 0.7f, 1), 1, 3, 0);

                    scrollArea[i, l] = EditorGUILayout.BeginScrollView(scrollArea[i, l], GUILayout.Height(100));
                    texts[i, l] = GUILayout.TextArea(texts[i, l], GUILayout.Width(200), GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        GUI.EndGroup();
    }
}
