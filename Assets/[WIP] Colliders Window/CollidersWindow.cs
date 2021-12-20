using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CollidersWindow : EditorWindow
{
    List<GameObject> TraverseList = new List<GameObject>();
    Vector3 objectSizeAlert = new(0.25f, 0.25f, 0.25f);

    [MenuItem("Tools/Colliders Window")]
    static void Init()
    {
        CollidersWindow sizeWindow = GetWindow<CollidersWindow>("Colliders In Scene");
        sizeWindow.autoRepaintOnSceneChange = true;
        sizeWindow.Show();
    }
    void OnGUI()
    {
        using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(500), GUILayout.ExpandWidth(false)))
        {
            GUILayout.Label("Min Object Size");
            objectSizeAlert = EditorGUILayout.Vector3Field("", objectSizeAlert); 
        }

        if (GUILayout.Button("Update"))
        {
            TraverseList = StartTraverse();
        }

        for (int i = 0; i < TraverseList.Count; i++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (!TraverseList[i].TryGetComponent(out Collider collider))
                {
                    continue;
                }

                using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(300), GUILayout.ExpandWidth(false)))
                {
                    EditorGUILayout.ObjectField(collider, typeof(Collider), true);
                }

                if (!TraverseList[i].TryGetComponent(out MeshFilter mf))
                {
                    continue;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    Mesh mesh = mf.sharedMesh;
                    if (mesh)
                    {
                        Vector3 size = Vector3.Scale(mesh.bounds.size, TraverseList[i].transform.localScale);
                        bool condition = size.x < objectSizeAlert.x && size.y < objectSizeAlert.y && size.z < objectSizeAlert.z;
                        string message = size + (condition ? " Object is too small" : "");

                        if (condition)
                            GUI.color = new Color(1, 0, 0);

                        GUILayout.Label(message);
                        GUI.color = Color.white;

                    }
                }

                using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(100), GUILayout.ExpandWidth(false)))
                {
                   // if (GUILayout.Button("Remove"))
                   // {
                   //
                   // }
                }
            }
        }
    }

    void Traverse(GameObject obj, ref List<GameObject> t)
    {
        t.Add(obj);
        foreach (Transform child in obj.transform)
        {
            Traverse(child.gameObject, ref t);
        }
    }
    List<GameObject> StartTraverse()
    {
        List<GameObject> traverse = new List<GameObject>();
        foreach (GameObject obj in FindObjectsOfType<GameObject>().OrderBy(m => m.transform.GetSiblingIndex()).ToArray())
        {
            if (obj.transform.parent == null)
            {
                Traverse(obj, ref traverse);
            }
        }

        return traverse;
    }
}