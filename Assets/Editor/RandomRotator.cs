using UnityEditor;
using UnityEngine;

public class RandomRotator : EditorWindow
{
    [SerializeField] int numOfSelectObj;
    [SerializeField] bool rotateX = true;
    [SerializeField] bool rotateY = true;
    [SerializeField] bool rotateZ = true;

    [MenuItem("Tools/Random Rotator")]
    static void Create()
    {
        RandomRotator window = GetWindowWithRect<RandomRotator>(new Rect(0, 0, 270, 180), true, "Random Rotator");
        window.Show();
    }

    void SetRot()
    {
        Transform[] group = Selection.GetTransforms(SelectionMode.Unfiltered);

        Vector3 newRot;
        Undo.RecordObjects(group, "Random Rotated objects");
        for (int i = 0; i < group.Length; i++)
        {
            newRot.x = rotateX ? Random.Range(0f, 359f) : group[i].eulerAngles.x;
            newRot.y = rotateY ? Random.Range(0f, 359f) : group[i].eulerAngles.y;
            newRot.z = rotateZ ? Random.Range(0f, 359f) : group[i].eulerAngles.z;

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
        GUILayout.Label(new GUIContent("Rotate selected objects with random values."));

        GUILayout.Space(10);

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
        GUILayout.Label("Selected: ");
        GUI.contentColor = Color.green;
        GUILayout.Label(numOfSelectObj.ToString());
        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        rotateX = GUILayout.Toggle(rotateX, "Rotate X");
        rotateY = GUILayout.Toggle(rotateY, "Rotate Y");
        rotateZ = GUILayout.Toggle(rotateZ, "Rotate Z");

        GUILayout.Space(20);

        if (GUILayout.Button("Rotate Selected"))
        {
            SetRot();
        }
    }
}
