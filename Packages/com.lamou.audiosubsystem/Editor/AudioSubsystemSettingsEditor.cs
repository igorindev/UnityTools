using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AudioSubsystem.Editor
{
    [CustomEditor(typeof(AudioSubsystemSettings))]
    public class AudioSubsystemSettingsEditor : UnityEditor.Editor
    {
        AudioSubsystemSettings _audioSubsystemSettings;

        private void OnEnable()
        {
            _audioSubsystemSettings = (AudioSubsystemSettings)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

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
    }
}
