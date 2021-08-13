using UnityEngine;

[CreateAssetMenu(fileName = "Scene Loader Obj", menuName = "ScriptableObjects/Toolbar/Scene Loader")]
public class SceneLoaderObj : ToolbarTool
{
    public override void Perform()
    {
        SceneLoader.ShowWindow();
    }
}
