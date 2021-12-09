using UnityEngine;
using UnityEditor;

public class PlaymodeEjectSettings : ScriptableObject
{
    [SerializeField] static bool showCursor = false;
    [SerializeField] static bool pauseTimeScale = false;

    static PlaymodeEject playmodeEject;

   // [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        Debug.Log("After Scene is loaded and game is running");

        GameObject _object = new GameObject("Playmode Eject");
        playmodeEject = _object.AddComponent<PlaymodeEject>();
        playmodeEject.tag = "EditorOnly";

        playmodeEject.showCursor = pauseTimeScale;

        playmodeEject.pauseTimeScale = pauseTimeScale;

        DontDestroyOnLoad(playmodeEject);
        
    }
}
