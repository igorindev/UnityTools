using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Actions/Load Scene")]
public class LoadSceneActionSO : ActionSO
{
    public LoadSceneMode sceneMode = LoadSceneMode.Single;
    public string sceneName;
    public bool loadAssync;

    public override void Execute()
    {
        if (loadAssync)
        {
            var assyncOP = SceneManager.LoadSceneAsync(sceneName, sceneMode);
            assyncOP.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene(sceneName, sceneMode);
        }
        base.Execute();
    }

    public override void DebugExecuteAction()
    {
        Debug.LogWarning($"{typeof(LoadSceneActionSO)} Action: {sceneName} Async: {loadAssync}");
    }
}