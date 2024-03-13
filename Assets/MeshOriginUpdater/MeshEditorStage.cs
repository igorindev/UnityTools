using System;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MeshEditorStage : PreviewSceneStage
{
    public Action onCloseStage;

    protected override GUIContent CreateHeaderContent()
    {
        return new GUIContent("Mesh Editor");
    }

    protected override bool OnOpenStage()
    {
        return base.OnOpenStage();
    }

    protected override void OnCloseStage()
    {
        base.OnCloseStage();

        onCloseStage?.Invoke();
    }
}
