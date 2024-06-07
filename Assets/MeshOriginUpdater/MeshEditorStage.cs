using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MeshEditorStage : PreviewSceneStage
{
    public Action onCloseStage;
    ToolbarMeshPivot toolbarMeshPivot;

    protected override GUIContent CreateHeaderContent()
    {
        return new GUIContent("Mesh Editor");
    }

    protected override bool OnOpenStage()
    {
        base.OnOpenStage();

        SceneView.lastActiveSceneView.sceneLighting = true;
        SceneView.lastActiveSceneView.sceneViewState.showSkybox = false;

        GameObject gameObject = new GameObject("Scene Light");
        gameObject.hideFlags = HideFlags.HideInHierarchy;
        Light light = gameObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(new Vector3(50, -30, 0));
        StageUtility.PlaceGameObjectInCurrentStage(gameObject);

        ToolbarMeshPivot toolbarMeshPivot = new ToolbarMeshPivot();
        SceneView.AddOverlayToActiveView(toolbarMeshPivot);
        return true;
    }

    protected override void OnCloseStage()
    {
        base.OnCloseStage();
        SceneView.lastActiveSceneView.sceneLighting = true;
        SceneView.lastActiveSceneView.sceneViewState.showSkybox = true;
        SceneView.RemoveOverlayFromActiveView(toolbarMeshPivot);

        onCloseStage?.Invoke();
    }
}
