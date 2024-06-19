using UnityEditor;
using UnityEngine;

namespace SceneLoader
{
    [CustomEditor(typeof(SceneLoaderGroup))]
    public class SceneLoaderGroupCustomEditor : Editor
    {
        SceneLoaderGroup sceneLoaderGroup;
        void OnEnable()
        {
            sceneLoaderGroup = target as SceneLoaderGroup;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(new GUIContent("Get All Scenes")))
            {
                sceneLoaderGroup.GetAllScenes();
            }

            base.OnInspectorGUI();
        }
    }
}