using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

public class CollidersExplorer : EditorWindow
{
    List<Collider> TraverseList = new List<Collider>();
    Vector3 objectSizeAlert = new Vector3(0.25f, 0.25f, 0.25f);

    Vector2 scroll;
    int selected;
    int wasSelected;
    string toSearch;
    string searchName;
    GUIContent[] textures;
     
    static CollidersExplorer sizeWindow;

    [MenuItem("Tools/Colliders Explorer...")]
    static void Init()
    {
        sizeWindow = GetWindow<CollidersExplorer>("Colliders Explorer");
        sizeWindow.autoRepaintOnSceneChange = true;
        sizeWindow.titleContent = new GUIContent("Colliders Explorer", EditorGUIUtility.IconContent("d_Search Icon").image);
        sizeWindow.minSize = new Vector2(431.00f, 400f);
        sizeWindow.Show();
    }

    void OnEnable()
    {
        TraverseList = StartTraverse();
        wasSelected = selected = 0;
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
            if (wasSelected != selected)
            {
                wasSelected = selected;
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
            }
            GUILayout.FlexibleSpace();
        }
        GUILayout.Space(0);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        GUILayout.Space(1);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open Physics Debug"))
            {
                GetWindow<PhysicsDebugWindow>();
            }
        }
        GUILayout.Space(20);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("Min Object Size");
            objectSizeAlert = EditorGUILayout.Vector3Field("", objectSizeAlert);
            GUILayout.FlexibleSpace();
        }
        GUILayout.Space(4);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(10);
            //EditorGUILayout.LabelField("Search", GUILayout.Width(50));
            //searchName = EditorGUILayout.TextField(searchName);
        }

        using (var scrollScope = new EditorGUILayout.ScrollViewScope(scroll, EditorStyles.helpBox))
        {
            EditorGUILayout.Space(3);
            scroll = scrollScope.scrollPosition;

            for (int i = 0; i < TraverseList.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (TraverseList[i] == null) { continue; }
                    Collider[] colliders = TraverseList[i].GetComponents<Collider>();
                    if (colliders.Length == 0) { continue; }

                    using (new EditorGUILayout.VerticalScope())
                    {
                        bool none = true;
                        int count = 0;
                        foreach (Collider item in colliders)
                        {
                            if (toSearch == "Sphere")
                            {
                                if (item.GetType() != typeof(SphereCollider))
                                {
                                    continue;
                                }
                            }
                            else if (toSearch == "Capsule")
                            {
                                if (item.GetType() != typeof(CapsuleCollider))
                                {
                                    continue;
                                }
                            }
                            else if (toSearch == "Box")
                            {
                                if (item.GetType() != typeof(BoxCollider))
                                {
                                    continue;
                                }
                            }
                            else if (toSearch == "Mesh")
                            {
                                if (item.GetType() != typeof(MeshCollider))
                                {
                                    continue;
                                }
                            }

                            using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(500)))
                            {
                                GUILayout.Space(3);
                                if (count > 0)
                                {
                                    GUI.color = Color.yellow;
                                    GUILayout.Label("Same as Above", BackgroundStyle.Get(), GUILayout.Width(110));
                                    GUI.color = Color.white;
                                }
                                if (item.isTrigger)
                                {
                                    GUI.color = Color.green;
                                    GUILayout.Label("Trigger", BackgroundStyle.Get(), GUILayout.Width(55));
                                    GUI.color = Color.white;
                                }

                                EditorGUILayout.ObjectField(item, typeof(Collider), true);
                                count += 1;
                                none = false;
                            }
                        }

                        if (none)
                        {
                            continue;
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (!TraverseList[i].TryGetComponent(out MeshFilter mf))
                        {
                            GUI.color = Color.cyan;
                            GUILayout.Label("No Mesh Filter", BackgroundStyle.Get(), GUILayout.MinWidth(110));
                            GUI.color = Color.white;
                        }
                        else
                        {
                            Mesh mesh = mf.sharedMesh;
                            if (mesh)
                            {
                                Vector3 size = Vector3.Scale(mesh.bounds.size, TraverseList[i].transform.localScale);
                                bool condition = size.x < objectSizeAlert.x && size.y < objectSizeAlert.y && size.z < objectSizeAlert.z;
                                string message = (condition ? " Too small" : size.ToString());

                                if (condition)
                                    GUI.color = Color.red;

                                GUILayout.Label(message, BackgroundStyle.Get(), GUILayout.MinWidth(110));
                                GUI.color = Color.white;
                            }
                        }
                    }

                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.Space(2f);
            }
        }
        EditorGUILayout.Space(5f);
    }

    void Traverse(GameObject obj, ref List<GameObject> t)
    {
        t.Add(obj);
        foreach (Transform child in obj.transform)
        {
            Traverse(child.gameObject, ref t);
        }
    }
    List<Collider> StartTraverse(string type = "")
    {
        toSearch = type;

        List<GameObject> traverse = new List<GameObject>();
        List<Collider> traversedColliders = new List<Collider>();
        foreach (GameObject obj in FindObjectsOfType<GameObject>().OrderBy(m => m.transform.GetSiblingIndex()).ToArray())
        {
            if (obj.transform.parent == null)
            {
                Traverse(obj, ref traverse);
            }
        }

        for (int i = 0; i < traverse.Count; i++)
        {
            if (traverse[i].TryGetComponent(out Collider c))
                traversedColliders.Add(c);
        }

        return traversedColliders;
    }
    public static class BackgroundStyle
    {
        private static GUIStyle style = new GUIStyle(GUI.skin.button);
        public static GUIStyle Get()
        {
            return style;
        }
    }
}
