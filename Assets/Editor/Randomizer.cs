using UnityEditor;
using UnityEngine;

public class Randomizer : EditorWindow
{
    [SerializeField] int numOfSelectObj;
    [SerializeField] bool randomizeX = true;
    [SerializeField] bool randomizeY = true;
    [SerializeField] bool randomizeZ = true;

    [MenuItem("Tools/Randomizer")]
    static void Create()
    {
        var window = GetWindow<Randomizer>("Randomizer");
        window.position = new Rect(0, 0, 270, 300);
        window.Show();
    }

    void SetRot()
    {
        Transform[] group = Selection.GetTransforms(SelectionMode.TopLevel);

        Vector3 newRot;

        for (int i = 0; i < group.Length; i++)
        {
            newRot.x = randomizeX ? Random.Range(0f, 359f) : group[i].eulerAngles.x;
            newRot.y = randomizeY ? Random.Range(0f, 359f) : group[i].eulerAngles.y;
            newRot.z = randomizeZ ? Random.Range(0f, 359f) : group[i].eulerAngles.z;

            group[i].eulerAngles = newRot;
        }
    }

    void OnInspectorUpdate()
    {
        numOfSelectObj = Selection.GetTransforms(SelectionMode.TopLevel).Length;
    }

    void OnGUI()
    {
        GUILayout.Label("");
        GUILayout.Label(new GUIContent("Randomize rotation of selected objects."));

        GUILayout.Space(10);

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
        GUILayout.Label("Number of Selected: ");
        GUI.contentColor = Color.green;
        GUILayout.Label(numOfSelectObj.ToString());
        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        randomizeX = GUILayout.Toggle(randomizeX, "Randomize X");
        randomizeY = GUILayout.Toggle(randomizeY, "Randomize Y");
        randomizeZ = GUILayout.Toggle(randomizeZ, "Randomize Z");

        GUILayout.Space(20);

        if (GUILayout.Button("Randomize Selected"))
        {
            SetRot();
        }
    }
}
