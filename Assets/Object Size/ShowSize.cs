using UnityEditor;
using UnityEngine;

class ShowSize : EditorWindow
{
    [MenuItem("Tools/ShowObjectSize")]
    static void Init()
    {
        ShowSize sizeWindow = GetWindow<ShowSize>();
        sizeWindow.autoRepaintOnSceneChange = true;
        sizeWindow.Show();
    }
    void OnGUI()
    {
        GameObject thisObject = Selection.activeObject as GameObject;
        if (thisObject == null)
        {
            return;
        }

        MeshFilter mf = thisObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (mf == null)
        { return; }

        Mesh mesh = mf.sharedMesh;
        if (mesh == null)
        { return; }

        Vector3 size = Vector3.Scale(mesh.bounds.size, thisObject.transform.localScale);
        GUILayout.Label("" + size);
    }
    void OnSelectionChange()
    {
        if (Selection.activeGameObject != null)
        {
            Repaint();
        }
    }
}

