using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MeshViewStage : PreviewSceneStage
{
    public Action onCloseStage;

    protected override GUIContent CreateHeaderContent()
    {
        return new GUIContent("Mesh View", EditorGUIUtility.IconContent("d_Mesh Icon").image);
    }

    protected override bool OnOpenStage()
    {
        base.OnOpenStage();

        Selection.SetActiveObjectWithContext(AssetDatabase.LoadAssetAtPath<ScriptableObject>("Packages/com.lamou.meshpivotupdater/Editor/Inspector/MeshPivotInspector.asset"), null);
        EditorWindow.GetWindow(Type.GetType("UnityEditor.InspectorWindow, UnityEditor"));
        ActiveEditorTracker.sharedTracker.isLocked = true;

        SessionState.SetBool("sceneLighting", SceneView.lastActiveSceneView.sceneLighting);
        SessionState.SetBool("showSkybox", SceneView.lastActiveSceneView.sceneViewState.showSkybox);

        SceneView.lastActiveSceneView.sceneLighting = true;
        SceneView.lastActiveSceneView.sceneViewState.showSkybox = false;

        GameObject gameObject = new("Scene Light")
        {
            hideFlags = HideFlags.HideInHierarchy
        };

        Light light = gameObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(new Vector3(50, -30, 0));

        StageUtility.PlaceGameObjectInCurrentStage(gameObject);

        return true;
    }

    protected override void OnCloseStage()
    {
        base.OnCloseStage();
        SceneView.lastActiveSceneView.sceneLighting = SessionState.GetBool("sceneLighting", true);
        SceneView.lastActiveSceneView.sceneViewState.showSkybox = SessionState.GetBool("showSkybox", true);

        EditorApplication.ExecuteMenuItem("Window/General/Inspector");
        ActiveEditorTracker.sharedTracker.isLocked = false;

        onCloseStage?.Invoke();
    }
}
