using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

internal class PlayFromFirstSceneShortcut
{
    [InitializeOnLoadMethod]
    public static void Init()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    private const string LastSceneKey = "LastActiveScene Index";

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode && EditorPrefs.HasKey(LastSceneKey))
        {
            EditorSceneManager.OpenScene(EditorPrefs.GetString(LastSceneKey));
            EditorPrefs.DeleteKey(LastSceneKey);
        }
    }

    [MenuItem("Tools/Play At First Scene _F5")]
    private static void PlayAtFirstScene()
    {
        if (!EditorApplication.isPlaying)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorPrefs.SetString(LastSceneKey, SceneManager.GetActiveScene().path);
                EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(0));
                EditorApplication.EnterPlaymode();
            }
        }
    }

    [MenuItem("Tools/Play At First Scene _F5", validate = true)]
    private static bool PlayAtFirstSceneValidation()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }
}
