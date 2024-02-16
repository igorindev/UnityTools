using UnityEditor;
using UnityEngine;

namespace SceneLoader
{
    public class SceneLoaderGroup : ScriptableObject
    {
        [System.Serializable]
        public struct ScenesCategories
        {
            [Scene] public string category;
            [Scene] public string[] scenes;
        }

        [Header("Categories")]
        [Scene] public string[] scenes;
        ScenesCategories[] scenesCategories;

        public ScenesCategories[] ScenesCategoriesGroups { get => scenesCategories; set => scenesCategories = value; }

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