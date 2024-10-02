using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshPivotUpdater
{
    [CustomEditor(typeof(MeshPivotInspector))]
    public class MeshPivotInspectorEditor : Editor
    {
        private static MeshViewStage stage;
        private static GameObject stageGameObject;
        private static Mesh meshInstanceInStage;

        private Vector3 handlerLocalPosition;
        private Vector3 objectPosition;
        private Vector3 handlerPosition;
        private Vector3 inspectorPosition;

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
                ReturnToMainStage();

                OpenStage(obj);
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ReturnToMainStage()
        {
            if (StageUtility.GetCurrentStage() is MeshViewStage)
            {
                StageUtility.GoToMainStage();
            }
        }

        private void OnEnable()
        {
            if (stageGameObject)
            {
                SceneView.duringSceneGui += OnScene;
                objectPosition = stageGameObject.transform.position;
            }
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

            if (GUILayout.Button("Set Pivot in Center of Mass"))
            {
                handlerPosition = meshInstanceInStage.bounds.center;
                handlerLocalPosition = handlerPosition - objectPosition;
                UpdateVertices();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Save as New Mesh"))
            {
                if (MeshSaverEditor.SaveMeshNewInstance(meshInstanceInStage, out Mesh newInstance))
                {
                    OpenMeshAsset(newInstance);
                }
            }
        }

        private static void OpenStage(Mesh obj)
        {
            Tools.hidden = true;

            stage = CreateInstance<MeshViewStage>();
            stage.OnCloseStageCallback += OnCloseStage;

            if (!EditorWindow.HasOpenInstances<SceneView>())
            {
                EditorWindow.GetWindow<SceneView>();
            }

            EditorWindow.FocusWindowIfItsOpen<SceneView>();

            StageUtility.GoToStage(stage, true);

            meshInstanceInStage = CopyMesh(obj);
            stageGameObject = new GameObject(meshInstanceInStage.name);
            MeshFilter mf = stageGameObject.AddComponent<MeshFilter>();
            MeshRenderer mr = stageGameObject.AddComponent<MeshRenderer>();

            int amount = meshInstanceInStage.subMeshCount;
            Material[] group = new Material[amount];

            var rp = GraphicsSettings.currentRenderPipeline;
            string shaderName = (rp != null, rp.GetType().Name) switch
            {
                (true, "HDRenderPipelineAsset") => "HDRP/Lit",
                (true, "UniversalRenderPipelineAsset") => "Universal Render Pipeline/Lit",
                _ => "Standard",
            };

            Shader shader = Shader.Find(shaderName);
            Material createdMaterial = new(shader);
            for (int i = 0; i < amount; i++)
            {
                group[i] = createdMaterial;
            }

            mf.sharedMesh = meshInstanceInStage;
            mr.sharedMaterials = group;

            StageUtility.PlaceGameObjectInCurrentStage(stageGameObject);
            Selection.activeGameObject = stageGameObject;

            SceneView.FrameLastActiveSceneView();
        }

        private static void OnCloseStage()
        {
            stage.OnCloseStageCallback -= OnCloseStage;

            DestroyImmediate(meshInstanceInStage);
            DestroyImmediate(stageGameObject);

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

        private void UpdateVertices()
        {
            Vector3[] objectVerts = meshInstanceInStage.vertices;

            for (int i = 0; i < objectVerts.Length; i++)
            {
                objectVerts[i] -= handlerLocalPosition;
            }

            meshInstanceInStage.vertices = objectVerts;

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
}