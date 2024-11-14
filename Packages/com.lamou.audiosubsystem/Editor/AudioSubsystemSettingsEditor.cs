using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AudioSubsystem.Editor
{
    [CustomEditor(typeof(AudioSubsystemSettings))]
    public class AudioSubsystemSettingsEditor : UnityEditor.Editor
    {
        private AudioSubsystemSettings _audioSubsystemSettings;

        private void OnEnable()
        {
            _audioSubsystemSettings = (AudioSubsystemSettings)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Get All Audios"))
            {
                GetAllAudios();
            }

            if (GUILayout.Button("Generate Constants"))
            {
                GenerateNames();
            }
        }

        private void GenerateNames()
        {
            List<string> names = new();
            foreach (AudioData item in _audioSubsystemSettings.Audios)
            {
                if (!names.Contains(item.name))
                {
                    names.Add(item.name);
                }
            }

            GenerateEnum.CreateConsts("AudioNames", names);
        }

        private void GetAllAudios()
        {
            _audioSubsystemSettings.Audios = FindAssetsByType<AudioData>().ToList();
            _audioSubsystemSettings.Audios.OrderBy(audio => audio.name);
        }

        private static IEnumerable<T> FindAssetsByType<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (string t in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(t);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    yield return asset;
                }
            }
        }
    }
}
