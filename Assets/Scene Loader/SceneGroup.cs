using UnityEditor;
using UnityEngine;

namespace SceneLoader
{
    [CreateAssetMenu(fileName = "SceneGroupEditor", menuName = "ScriptableObjects/Scene Group Editor")]
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
}