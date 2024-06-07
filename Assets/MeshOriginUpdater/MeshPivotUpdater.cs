using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MeshPivotUpdater : EditorWindow
{
    GameObject stageGameObject;

    static Mesh meshInstanceAtStage;

    Vector3 handlerLocalPosition;
    Vector3 objectPosition;
    Vector3 handlerPosition;
    Vector3 inspectorPosition;

    MeshEditorStage stage;

    static MeshPivotUpdater window;

    Coordinates coordinates;

    enum Coordinates
    {
        World,
        Local
    }

    static void OpenWindow()
    {
        window = GetWindow<MeshPivotUpdater>();
        window.titleContent = new GUIContent("Mesh Pivot Updater");
    }

    [OnOpenAsset(2)]
    public static bool OpenAsset(int instanceId, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceId);

        if (window)
            window.Close();

        if (obj is Mesh mesh)
        {
            meshInstanceAtStage = CopyMesh(mesh);
            OpenWindow();
            return true;
        }
        return false;
    }

    void OnEnable()
    {
        OpenStage();
        SceneView.duringSceneGui += OnSceneGUI;
        Tools.hidden = true;
    }

    void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;

        stage.onCloseStage -= OnCloseStage;
        if (StageUtility.GetCurrentStage() is MeshEditorStage)
        {
            StageUtility.GoToMainStage();
        }

        stageGameObject = null;
        Tools.hidden = false;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUI.BeginChangeCheck();
        coordinates = (Coordinates)EditorGUILayout.EnumPopup("Coordinate", coordinates);
        if (coordinates == Coordinates.World)
        {
            inspectorPosition = EditorGUILayout.Vector3Field("Position", inspectorPosition);
        }
        else
        {
            inspectorPosition = EditorGUILayout.Vector3Field("Position", inspectorPosition - objectPosition) + objectPosition;
        }

        if (EditorGUI.EndChangeCheck())
        {
            handlerPosition = inspectorPosition;
        }

        if (GUILayout.Button("Update Pivot"))
        {
            UpdateVertices();
        }

        if (GUILayout.Button("Save and Override Mesh"))
        {

        }

        EditorGUILayout.EndVertical();
    }

    void OnCloseStage()
    {
        Close();
    }

    void OpenStage()
    {
        stage = CreateInstance<MeshEditorStage>();
        stage.onCloseStage += OnCloseStage;

        StageUtility.GoToStage(stage, true);

        stageGameObject = new GameObject(meshInstanceAtStage.name);
        MeshFilter mf = stageGameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = stageGameObject.AddComponent<MeshRenderer>();

        int amount = meshInstanceAtStage.subMeshCount;
        Material[] group = new Material[amount];
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        Material createdMaterial = new Material(shader);
        for (int i = 0; i < amount; i++)
        {
            group[i] = createdMaterial;
        }

        mf.sharedMesh = meshInstanceAtStage;
        mr.sharedMaterials = group;

        objectPosition = stageGameObject.transform.position;
        StageUtility.PlaceGameObjectInCurrentStage(stageGameObject);
        Selection.activeGameObject = stageGameObject;
        SceneView.FrameLastActiveSceneView();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (stageGameObject)
        {
            objectPosition = stageGameObject.transform.position;

            static float UpdateSize(float fixedValue, Vector3 target, Vector3 camera, float fov = 1)
            {
                float distance = (camera - target).magnitude;
                var size = distance * fixedValue * fov;
                return size;
            }

            float size = UpdateSize(0.0002f, objectPosition, sceneView.camera.transform.position, sceneView.camera.fieldOfView);

            Handles.color = Color.yellow;
            Handles.DrawSolidDisc(objectPosition, -sceneView.camera.transform.forward, size);
            Handles.color = Color.black;
            Handles.DrawWireDisc(objectPosition, -sceneView.camera.transform.forward, size, 0.08f);
            Handles.color = Color.white;

            EditorGUI.BeginChangeCheck();

            if (StageUtility.GetCurrentStage() is MeshEditorStage)
            {
                handlerPosition = Handles.DoPositionHandle(handlerPosition, Quaternion.identity);
            }

            if (DetectChange(inspectorPosition, handlerPosition))
            {
                inspectorPosition = handlerPosition;
                Repaint();
            }

            handlerLocalPosition = handlerPosition - objectPosition;
        }
    }

    void UpdateVertices()
    {
        Vector3[] objectVerts = meshInstanceAtStage.vertices;

        for (int i = 0; i < objectVerts.Length; i++)
        {
            objectVerts[i] -= handlerLocalPosition;
        }

        meshInstanceAtStage.vertices = objectVerts;

        stageGameObject.transform.position += handlerLocalPosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Mesh CopyMesh(Mesh mesh)
    {
        return new Mesh()
        {
            name = mesh.name + " (Instance)",
            vertices = mesh.vertices,
            triangles = mesh.triangles,
            normals = mesh.normals,
            tangents = mesh.tangents,
            bounds = mesh.bounds,
            uv = mesh.uv
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool DetectChange(Vector3 v1, Vector3 v2)
    {
        return !Mathf.Approximately(v1.x, v2.x) || !Mathf.Approximately(v1.y, v2.y) || !Mathf.Approximately(v1.z, v2.z);
    }
}
