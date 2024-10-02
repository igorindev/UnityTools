using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MeshPivotUpdater
{
    public class MeshViewStage : PreviewSceneStage
    {
        public event Action OnCloseStageCallback;

        private MeshPivotInspector meshPivotInspectorInstance;

        protected override GUIContent CreateHeaderContent()
        {
            return new GUIContent("Mesh View", EditorGUIUtility.IconContent("d_Mesh Icon").image);
        }

        protected override bool OnOpenStage()
        {
            base.OnOpenStage();
            meshPivotInspectorInstance = CreateInstance<MeshPivotInspector>();
            meshPivotInspectorInstance.name = "Mesh Pivot Inspector Instance";
            Selection.SetActiveObjectWithContext(meshPivotInspectorInstance, null);
            EditorWindow.GetWindow(Type.GetType("UnityEditor.InspectorWindow, UnityEditor"));
            ActiveEditorTracker.sharedTracker.isLocked = true;

            SessionState.SetBool("sceneLighting", SceneView.lastActiveSceneView.sceneLighting);
            SessionState.SetBool("showSkybox", SceneView.lastActiveSceneView.sceneViewState.showSkybox);

            SceneView.lastActiveSceneView.sceneLighting = true;
            SceneView.lastActiveSceneView.sceneViewState.showSkybox = false;

            GameObject lightObj1 = new("Scene Light")
            {
                hideFlags = HideFlags.HideInHierarchy
            };
            GameObject lightObj2 = new("Scene Light")
            {
                hideFlags = HideFlags.HideInHierarchy
            };

            Light light = lightObj1.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(new Vector3(50, -30, 0));

            light = lightObj2.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.5f;
            light.transform.rotation = Quaternion.Euler(new Vector3(240, -30, 0));

            StageUtility.PlaceGameObjectInCurrentStage(lightObj1);
            StageUtility.PlaceGameObjectInCurrentStage(lightObj2);

            return true;
        }

        protected override void OnCloseStage()
        {
            base.OnCloseStage();
            if (SceneView.lastActiveSceneView)
            {
                SceneView.lastActiveSceneView.sceneLighting = SessionState.GetBool("sceneLighting", true);
                SceneView.lastActiveSceneView.sceneViewState.showSkybox = SessionState.GetBool("showSkybox", true);
            }

            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
            ActiveEditorTracker.sharedTracker.isLocked = false;

            if (meshPivotInspectorInstance != null)
            {
                DestroyImmediate(meshPivotInspectorInstance);
            }

            OnCloseStageCallback?.Invoke();
        }
    }
}