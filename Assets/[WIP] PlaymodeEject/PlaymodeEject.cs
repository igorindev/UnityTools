using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlaymodeEject : MonoBehaviour
{
    public bool showCursor = false;
    public bool pauseTimeScale = false;

    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            EditorApplication.isPaused = !EditorApplication.isPaused;
        }
    }
}
