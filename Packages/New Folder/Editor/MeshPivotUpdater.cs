using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(MeshPivotInspector))]
public class MeshPivotUpdater : Editor
{
    private static GameObject stageGameObject;

    private static Mesh meshInstanceAtStage;

    private static Vector3 handlerLocalPosition;
    private static Vector3 objectPosition;
    private static Vector3 handlerPosition;
    private static Vector3 inspectorPosition;

    private static MeshViewStage stage;

    private Coordinates coordinates;

    private enum Coordinates : byte
    {
        World,
        Local
    }

    public static void OpenMeshAsset(Mesh obj)
    {
        if (obj)
        {
            meshInstanceAtStage = CopyMesh(obj);

            if (StageUtility.GetCurrentStage() is MeshViewStage)
            {
                StageUtility.GoToMainStage();
            }

            OpenStage();
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnScene;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnScene;
    }

    public override void OnInspectorGUI()
    {
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
    }

    private static void OpenStage()
    {
        Tools.hidden = true;

        stage = CreateInstance<MeshViewStage>();
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

    private static void OnCloseStage()
    {
        stage.onCloseStage -= OnCloseStage;

        if (StageUtility.GetCurrentStage() is MeshViewStage)
        {
            StageUtility.GoToMainStage();
        }

        stageGameObject = null;
        Tools.hidden = false;
    }

    private void OnScene(SceneView sceneView)
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

            if (StageUtility.GetCurrentStage() is MeshViewStage)
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

    private static void UpdateVertices()
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
    private static Mesh CopyMesh(Mesh mesh)
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
    private static bool DetectChange(Vector3 v1, Vector3 v2)
    {
        return !Mathf.Approximately(v1.x, v2.x) || !Mathf.Approximately(v1.y, v2.y) || !Mathf.Approximately(v1.z, v2.z);
    }
}
