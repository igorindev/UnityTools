using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneGroup", menuName = "ScriptableObjects/Scene Group")]
public class SceneGroup : ScriptableObject
{
    [Scene] public string[] scenes;

#if UNITY_EDITOR
    [ContextMenu("Get All Scenes")]
    public void GetAllScenes()
    {
        string[] s = AssetDatabase.FindAssets("t:Scene");
        scenes = new string[s.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = AssetDatabase.GUIDToAssetPath(s[i]);
        }
    }
#endif
}
