using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CollidersExplorer : EditorWindow
{
    List<GameObject> TraverseList = new List<GameObject>();
    Vector3 objectSizeAlert = new(0.25f, 0.25f, 0.25f);

    Vector2 scroll;
    int selected;
    string toSearch;
    bool drawShapes;
    GUIContent[] textures;

    [MenuItem("Tools/Colliders Explorer")]
    static void Init()
    {
        CollidersExplorer sizeWindow = GetWindow<CollidersExplorer>("Colliders Explorer");
        sizeWindow.autoRepaintOnSceneChange = true;
        sizeWindow.titleContent = new GUIContent("Colliders Explorer", EditorGUIUtility.IconContent("BoxCollider Icon").image);
        sizeWindow.Show();
    }

    void OnEnable()
    {
        TraverseList = StartTraverse();

        textures = new GUIContent[]
        {
            new GUIContent("All"),
            new GUIContent(EditorGUIUtility.IconContent("SphereCollider Icon").image),
            new GUIContent(EditorGUIUtility.IconContent("CapsuleCollider Icon").image),
            new GUIContent(EditorGUIUtility.IconContent("BoxCollider Icon").image),
            new GUIContent(EditorGUIUtility.IconContent("MeshCollider Icon").image)
        };
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            selected = GUILayout.Toolbar(selected, textures, GUILayout.MaxHeight(30), GUILayout.Width(300));
            if (selected == 0)
            {
                TraverseList = StartTraverse();
            }
            else if (selected == 1)
            {
                TraverseList = StartTraverse("Sphere");
            }
            else if (selected == 2)
            {
                TraverseList = StartTraverse("Capsule");
            }
            else if (selected == 3)
            {
                TraverseList = StartTraverse("Box");
            }
            else if (selected == 4)
            {
                TraverseList = StartTraverse("Mesh");
            }
            GUILayout.FlexibleSpace();
        }
        GUILayout.Space(-10);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("__________________________________________________________________");
            GUILayout.FlexibleSpace();
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("Min Object Size");
            objectSizeAlert = EditorGUILayout.Vector3Field("", objectSizeAlert);
            GUILayout.FlexibleSpace();
        }
        GUILayout.Space(20);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(10);
            drawShapes = GUILayout.Toggle(drawShapes, "Draw Shapes");
        }
        GUILayout.Space(1);
        using (var scrollScope = new EditorGUILayout.ScrollViewScope(scroll, EditorStyles.helpBox))
        {
            scroll = scrollScope.scrollPosition;
            for (int i = 0; i < TraverseList.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (TraverseList[i] == null)
                    {
                        continue;
                    }
                    if (!TraverseList[i].TryGetComponent(out Collider collider))
                    {
                        continue;
                    }
                    if (toSearch == "Sphere")
                    {
                        if (!TraverseList[i].TryGetComponent(out SphereCollider s))
                        {
                            continue;
                        }
                    }
                    else if (toSearch == "Capsule")
                    {
                        if (!TraverseList[i].TryGetComponent(out CapsuleCollider c))
                        {
                            continue;
                        }
                    }
                    else if (toSearch == "Box")
                    {
                        if (!TraverseList[i].TryGetComponent(out BoxCollider b))
                        {
                            continue;
                        }
                    }
                    else if (toSearch == "Mesh")
                    {
                        if (!TraverseList[i].TryGetComponent(out MeshCollider b))
                        {
                            continue;
                        }
                    }
                    EditorGUILayout.Space(3);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.ObjectField(collider, typeof(Collider), true);
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (!TraverseList[i].TryGetComponent(out MeshFilter mf))
                        {
                            GUILayout.Label("(No Mesh Filter)");
                            EditorGUILayout.Space(5);
                            continue;
                        }

                        Mesh mesh = mf.sharedMesh;
                        if (mesh)
                        {
                            Vector3 size = Vector3.Scale(mesh.bounds.size, TraverseList[i].transform.localScale);
                            bool condition = size.x < objectSizeAlert.x && size.y < objectSizeAlert.y && size.z < objectSizeAlert.z;
                            string message = size + (condition ? " Object is too small" : "");

                            if (condition)
                                GUI.color = Color.red;

                            GUILayout.Label(message);
                            GUI.color = Color.white;
                        }
                    }
                    EditorGUILayout.Space(5);
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.Space(4f);
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
    List<GameObject> StartTraverse(string type = "")
    {
        toSearch = type;

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
